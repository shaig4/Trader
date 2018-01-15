CREATE TABLE [dbo].[Trades] (
    [price]      MONEY          NOT NULL,
    [volume]     MONEY          NOT NULL,
    [trade_time] DATETIME       NOT NULL,
    [is_buy]     BIT            NOT NULL,
    [is_market]  BIT            NOT NULL,
    [misc]       NVARCHAR (100) NULL, 
    [local_time] DATETIME NULL
);

