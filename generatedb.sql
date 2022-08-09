
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [MasterProducts] (
    [ID] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [Quantity] int NOT NULL,
    CONSTRAINT [PK_MasterProducts] PRIMARY KEY ([ID])
);
GO

CREATE TABLE [MoneyBox] (
    [ID] int NOT NULL IDENTITY,
    [Denomination] decimal(18,2) NOT NULL,
    [Quantity] int NOT NULL,
    CONSTRAINT [PK_MoneyBox] PRIMARY KEY ([ID])
);
GO

CREATE TABLE [Orders] (
    [ID] int NOT NULL IDENTITY,
    [OrderDate] datetime2 NOT NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([ID])
);
GO

CREATE TABLE [Payments] (
    [ID] int NOT NULL IDENTITY,
    [OrderID] int NOT NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_Payments] PRIMARY KEY ([ID])
);
GO

CREATE TABLE [OrderDetails] (
    [ID] int NOT NULL IDENTITY,
    [OrderID] int NOT NULL,
    [ProductID] int NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [Quantity] int NOT NULL,
    CONSTRAINT [PK_OrderDetails] PRIMARY KEY ([ID]),
    CONSTRAINT [FK_OrderDetails_Orders_OrderID] FOREIGN KEY ([OrderID]) REFERENCES [Orders] ([ID]) ON DELETE CASCADE
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Name', N'Price', N'Quantity') AND [object_id] = OBJECT_ID(N'[MasterProducts]'))
    SET IDENTITY_INSERT [MasterProducts] ON;
INSERT INTO [MasterProducts] ([ID], [Name], [Price], [Quantity])
VALUES (1, N'Mineral Water', 4000.0, 10),
(2, N'Soft Drink', 8000.0, 10),
(3, N'Milk', 6000.0, 10),
(4, N'Coffee', 10000.0, 10);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Name', N'Price', N'Quantity') AND [object_id] = OBJECT_ID(N'[MasterProducts]'))
    SET IDENTITY_INSERT [MasterProducts] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Denomination', N'Quantity') AND [object_id] = OBJECT_ID(N'[MoneyBox]'))
    SET IDENTITY_INSERT [MoneyBox] ON;
INSERT INTO [MoneyBox] ([ID], [Denomination], [Quantity])
VALUES (1, 100000.0, 10),
(2, 50000.0, 10),
(3, 20000.0, 100),
(4, 10000.0, 100),
(5, 5000.0, 100),
(6, 2000.0, 100),
(7, 1000.0, 100);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ID', N'Denomination', N'Quantity') AND [object_id] = OBJECT_ID(N'[MoneyBox]'))
    SET IDENTITY_INSERT [MoneyBox] OFF;
GO

CREATE INDEX [IX_OrderDetails_OrderID] ON [OrderDetails] ([OrderID]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20220809204326_InitDatabase', N'6.0.8');
GO

COMMIT;
GO


