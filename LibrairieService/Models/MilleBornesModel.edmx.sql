
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 02/04/2016 20:19:15
-- Generated from EDMX file: D:\projets\milleborne\LibrairieService\Models\MilleBornesModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [BDAlexMichael];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_DrawCardEvent_To_GameEvent]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DrawCardEvent] DROP CONSTRAINT [FK_DrawCardEvent_To_GameEvent];
GO
IF OBJECT_ID(N'[dbo].[FK_DrawCardEvent_To_Player]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DrawCardEvent] DROP CONSTRAINT [FK_DrawCardEvent_To_Player];
GO
IF OBJECT_ID(N'[dbo].[FK_GameEvent_To_Game]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GameEvent] DROP CONSTRAINT [FK_GameEvent_To_Game];
GO
IF OBJECT_ID(N'[dbo].[FK_GameMessage_To_Game]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GameMessage] DROP CONSTRAINT [FK_GameMessage_To_Game];
GO
IF OBJECT_ID(N'[dbo].[FK_GameMessage_To_Message]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GameMessage] DROP CONSTRAINT [FK_GameMessage_To_Message];
GO
IF OBJECT_ID(N'[dbo].[FK_LoggedInUser_To_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LoggedInUser] DROP CONSTRAINT [FK_LoggedInUser_To_User];
GO
IF OBJECT_ID(N'[dbo].[FK_Message_To_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Message] DROP CONSTRAINT [FK_Message_To_User];
GO
IF OBJECT_ID(N'[dbo].[FK_PlayCardEvent_To_DrawCardEvent]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PlayCardEvent] DROP CONSTRAINT [FK_PlayCardEvent_To_DrawCardEvent];
GO
IF OBJECT_ID(N'[dbo].[FK_PlayCardEvent_To_GameEvent]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PlayCardEvent] DROP CONSTRAINT [FK_PlayCardEvent_To_GameEvent];
GO
IF OBJECT_ID(N'[dbo].[FK_PlayerChangeEvent_To_GameEvent]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PlayerChangeEvent] DROP CONSTRAINT [FK_PlayerChangeEvent_To_GameEvent];
GO
IF OBJECT_ID(N'[dbo].[FK_PlayerChangeEvent_To_Player]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PlayerChangeEvent] DROP CONSTRAINT [FK_PlayerChangeEvent_To_Player];
GO
IF OBJECT_ID(N'[dbo].[FK_PlayerGame_To_Game]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PlayerGame] DROP CONSTRAINT [FK_PlayerGame_To_Game];
GO
IF OBJECT_ID(N'[dbo].[FK_PlayerGame_To_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PlayerGame] DROP CONSTRAINT [FK_PlayerGame_To_User];
GO
IF OBJECT_ID(N'[dbo].[FK_PlayerRoomState_To_Room]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PlayerRoomState] DROP CONSTRAINT [FK_PlayerRoomState_To_Room];
GO
IF OBJECT_ID(N'[dbo].[FK_PlayerRoomState_To_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PlayerRoomState] DROP CONSTRAINT [FK_PlayerRoomState_To_User];
GO
IF OBJECT_ID(N'[dbo].[FK_ReceiverUserId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PrivateMessage] DROP CONSTRAINT [FK_ReceiverUserId];
GO
IF OBJECT_ID(N'[dbo].[FK_Room_To_Game]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Room] DROP CONSTRAINT [FK_Room_To_Game];
GO
IF OBJECT_ID(N'[dbo].[FK_Room_To_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Room] DROP CONSTRAINT [FK_Room_To_User];
GO
IF OBJECT_ID(N'[dbo].[FK_RoomMessage_To_Message]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RoomMessage] DROP CONSTRAINT [FK_RoomMessage_To_Message];
GO
IF OBJECT_ID(N'[dbo].[FK_RoomMessage_To_Room]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RoomMessage] DROP CONSTRAINT [FK_RoomMessage_To_Room];
GO
IF OBJECT_ID(N'[dbo].[FK_SenderUserId_To_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PrivateMessage] DROP CONSTRAINT [FK_SenderUserId_To_User];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[DrawCardEvent]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DrawCardEvent];
GO
IF OBJECT_ID(N'[dbo].[Game]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Game];
GO
IF OBJECT_ID(N'[dbo].[GameEvent]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GameEvent];
GO
IF OBJECT_ID(N'[dbo].[GameMessage]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GameMessage];
GO
IF OBJECT_ID(N'[dbo].[LoggedInUser]', 'U') IS NOT NULL
    DROP TABLE [dbo].[LoggedInUser];
GO
IF OBJECT_ID(N'[dbo].[Message]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Message];
GO
IF OBJECT_ID(N'[dbo].[PlayCardEvent]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PlayCardEvent];
GO
IF OBJECT_ID(N'[dbo].[PlayerChangeEvent]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PlayerChangeEvent];
GO
IF OBJECT_ID(N'[dbo].[PlayerGame]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PlayerGame];
GO
IF OBJECT_ID(N'[dbo].[PlayerRoomState]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PlayerRoomState];
GO
IF OBJECT_ID(N'[dbo].[PrivateMessage]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PrivateMessage];
GO
IF OBJECT_ID(N'[dbo].[Room]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Room];
GO
IF OBJECT_ID(N'[dbo].[RoomMessage]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RoomMessage];
GO
IF OBJECT_ID(N'[dbo].[User]', 'U') IS NOT NULL
    DROP TABLE [dbo].[User];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Game'
CREATE TABLE [dbo].[Game] (
    [GameId] int IDENTITY(1,1) NOT NULL,
    [Token] uniqueidentifier  NOT NULL,
    [EndToken] uniqueidentifier  NULL,
    [EndDate] datetime  NULL,
    [EndReason] smallint  NULL,
    [StartDate] datetime  NOT NULL,
    [IsTakingDisconnectDecision] bit  NOT NULL
);
GO

-- Creating table 'GameEvent'
CREATE TABLE [dbo].[GameEvent] (
    [GameEventId] int IDENTITY(1,1) NOT NULL,
    [ServerEvent] bit  NOT NULL,
    [Date] datetime  NOT NULL,
    [Type] smallint  NOT NULL,
    [GameId] int  NOT NULL
);
GO

-- Creating table 'LoggedInUser'
CREATE TABLE [dbo].[LoggedInUser] (
    [UserId] int  NOT NULL,
    [Token] uniqueidentifier  NOT NULL,
    [LoginDate] datetime  NOT NULL,
    [LastHeartbeat] datetime  NOT NULL
);
GO

-- Creating table 'Message'
CREATE TABLE [dbo].[Message] (
    [MessageId] int IDENTITY(1,1) NOT NULL,
    [Date] datetime  NOT NULL,
    [Type] smallint  NOT NULL,
    [Content] nvarchar(255)  NOT NULL,
    [UserId] int  NOT NULL
);
GO

-- Creating table 'PlayerGame'
CREATE TABLE [dbo].[PlayerGame] (
    [GameId] int  NOT NULL,
    [UserId] int  NOT NULL,
    [Order] int  NOT NULL,
    [Team] int  NOT NULL,
    [LastHeartbeat] datetime  NOT NULL,
    [HasJoined] bit  NOT NULL
);
GO

-- Creating table 'PlayerRoomState'
CREATE TABLE [dbo].[PlayerRoomState] (
    [RoomId] int  NOT NULL,
    [UserId] int  NOT NULL,
    [IsReady] bit  NOT NULL,
    [Order] int  NOT NULL,
    [Team] int  NOT NULL,
    [LastHeartbeat] datetime  NOT NULL
);
GO

-- Creating table 'Room'
CREATE TABLE [dbo].[Room] (
    [RoomId] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [MasterUserId] int  NOT NULL,
    [Started] bit  NOT NULL,
    [GameInProgressId] int  NULL,
    [Token] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'User'
CREATE TABLE [dbo].[User] (
    [UserId] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(100)  NOT NULL,
    [PasswordHash] varchar(40)  NOT NULL,
    [EmailAddress] varchar(50)  NOT NULL
);
GO

-- Creating table 'PrivateMessage'
CREATE TABLE [dbo].[PrivateMessage] (
    [PrivateMessageId] int  NOT NULL,
    [SenderUserId] int  NOT NULL,
    [ReceiverUserId] int  NOT NULL,
    [Message] nvarchar(max)  NOT NULL,
    [Read] bit  NOT NULL,
    [SentTime] datetime  NOT NULL
);
GO

-- Creating table 'GameEvent_PlayerChangeEvent'
CREATE TABLE [dbo].[GameEvent_PlayerChangeEvent] (
    [NewPlayerId] int  NOT NULL,
    [GameEventId] int  NOT NULL
);
GO

-- Creating table 'GameEvent_DrawCardEvent'
CREATE TABLE [dbo].[GameEvent_DrawCardEvent] (
    [CardIndex] int  NOT NULL,
    [Token] uniqueidentifier  NOT NULL,
    [PlayerId] int  NOT NULL,
    [GameEventId] int  NOT NULL
);
GO

-- Creating table 'GameEvent_PlayCardEvent'
CREATE TABLE [dbo].[GameEvent_PlayCardEvent] (
    [TargetTeamIndex] int  NOT NULL,
    [DrawCardEventId] int  NOT NULL,
    [GameEventId] int  NOT NULL
);
GO

-- Creating table 'GameMessage'
CREATE TABLE [dbo].[GameMessage] (
    [Game_GameId] int  NOT NULL,
    [Message_MessageId] int  NOT NULL
);
GO

-- Creating table 'RoomMessage'
CREATE TABLE [dbo].[RoomMessage] (
    [Message_MessageId] int  NOT NULL,
    [Room_RoomId] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [GameId] in table 'Game'
ALTER TABLE [dbo].[Game]
ADD CONSTRAINT [PK_Game]
    PRIMARY KEY CLUSTERED ([GameId] ASC);
GO

-- Creating primary key on [GameEventId] in table 'GameEvent'
ALTER TABLE [dbo].[GameEvent]
ADD CONSTRAINT [PK_GameEvent]
    PRIMARY KEY CLUSTERED ([GameEventId] ASC);
GO

-- Creating primary key on [UserId] in table 'LoggedInUser'
ALTER TABLE [dbo].[LoggedInUser]
ADD CONSTRAINT [PK_LoggedInUser]
    PRIMARY KEY CLUSTERED ([UserId] ASC);
GO

-- Creating primary key on [MessageId] in table 'Message'
ALTER TABLE [dbo].[Message]
ADD CONSTRAINT [PK_Message]
    PRIMARY KEY CLUSTERED ([MessageId] ASC);
GO

-- Creating primary key on [GameId], [UserId] in table 'PlayerGame'
ALTER TABLE [dbo].[PlayerGame]
ADD CONSTRAINT [PK_PlayerGame]
    PRIMARY KEY CLUSTERED ([GameId], [UserId] ASC);
GO

-- Creating primary key on [RoomId], [UserId] in table 'PlayerRoomState'
ALTER TABLE [dbo].[PlayerRoomState]
ADD CONSTRAINT [PK_PlayerRoomState]
    PRIMARY KEY CLUSTERED ([RoomId], [UserId] ASC);
GO

-- Creating primary key on [RoomId] in table 'Room'
ALTER TABLE [dbo].[Room]
ADD CONSTRAINT [PK_Room]
    PRIMARY KEY CLUSTERED ([RoomId] ASC);
GO

-- Creating primary key on [UserId] in table 'User'
ALTER TABLE [dbo].[User]
ADD CONSTRAINT [PK_User]
    PRIMARY KEY CLUSTERED ([UserId] ASC);
GO

-- Creating primary key on [PrivateMessageId] in table 'PrivateMessage'
ALTER TABLE [dbo].[PrivateMessage]
ADD CONSTRAINT [PK_PrivateMessage]
    PRIMARY KEY CLUSTERED ([PrivateMessageId] ASC);
GO

-- Creating primary key on [GameEventId] in table 'GameEvent_PlayerChangeEvent'
ALTER TABLE [dbo].[GameEvent_PlayerChangeEvent]
ADD CONSTRAINT [PK_GameEvent_PlayerChangeEvent]
    PRIMARY KEY CLUSTERED ([GameEventId] ASC);
GO

-- Creating primary key on [GameEventId] in table 'GameEvent_DrawCardEvent'
ALTER TABLE [dbo].[GameEvent_DrawCardEvent]
ADD CONSTRAINT [PK_GameEvent_DrawCardEvent]
    PRIMARY KEY CLUSTERED ([GameEventId] ASC);
GO

-- Creating primary key on [GameEventId] in table 'GameEvent_PlayCardEvent'
ALTER TABLE [dbo].[GameEvent_PlayCardEvent]
ADD CONSTRAINT [PK_GameEvent_PlayCardEvent]
    PRIMARY KEY CLUSTERED ([GameEventId] ASC);
GO

-- Creating primary key on [Game_GameId], [Message_MessageId] in table 'GameMessage'
ALTER TABLE [dbo].[GameMessage]
ADD CONSTRAINT [PK_GameMessage]
    PRIMARY KEY CLUSTERED ([Game_GameId], [Message_MessageId] ASC);
GO

-- Creating primary key on [Message_MessageId], [Room_RoomId] in table 'RoomMessage'
ALTER TABLE [dbo].[RoomMessage]
ADD CONSTRAINT [PK_RoomMessage]
    PRIMARY KEY CLUSTERED ([Message_MessageId], [Room_RoomId] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [GameId] in table 'PlayerGame'
ALTER TABLE [dbo].[PlayerGame]
ADD CONSTRAINT [FK_PlayerGame_To_Game]
    FOREIGN KEY ([GameId])
    REFERENCES [dbo].[Game]
        ([GameId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [GameInProgressId] in table 'Room'
ALTER TABLE [dbo].[Room]
ADD CONSTRAINT [FK_Room_To_Game]
    FOREIGN KEY ([GameInProgressId])
    REFERENCES [dbo].[Game]
        ([GameId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Room_To_Game'
CREATE INDEX [IX_FK_Room_To_Game]
ON [dbo].[Room]
    ([GameInProgressId]);
GO

-- Creating foreign key on [UserId] in table 'LoggedInUser'
ALTER TABLE [dbo].[LoggedInUser]
ADD CONSTRAINT [FK_LoggedInUser_To_User]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[User]
        ([UserId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [UserId] in table 'Message'
ALTER TABLE [dbo].[Message]
ADD CONSTRAINT [FK_Message_To_User]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[User]
        ([UserId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Message_To_User'
CREATE INDEX [IX_FK_Message_To_User]
ON [dbo].[Message]
    ([UserId]);
GO

-- Creating foreign key on [UserId] in table 'PlayerGame'
ALTER TABLE [dbo].[PlayerGame]
ADD CONSTRAINT [FK_PlayerGame_To_User]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[User]
        ([UserId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PlayerGame_To_User'
CREATE INDEX [IX_FK_PlayerGame_To_User]
ON [dbo].[PlayerGame]
    ([UserId]);
GO

-- Creating foreign key on [RoomId] in table 'PlayerRoomState'
ALTER TABLE [dbo].[PlayerRoomState]
ADD CONSTRAINT [FK_PlayerRoomState_To_Room]
    FOREIGN KEY ([RoomId])
    REFERENCES [dbo].[Room]
        ([RoomId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [UserId] in table 'PlayerRoomState'
ALTER TABLE [dbo].[PlayerRoomState]
ADD CONSTRAINT [FK_PlayerRoomState_To_User]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[User]
        ([UserId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PlayerRoomState_To_User'
CREATE INDEX [IX_FK_PlayerRoomState_To_User]
ON [dbo].[PlayerRoomState]
    ([UserId]);
GO

-- Creating foreign key on [MasterUserId] in table 'Room'
ALTER TABLE [dbo].[Room]
ADD CONSTRAINT [FK_Room_To_User]
    FOREIGN KEY ([MasterUserId])
    REFERENCES [dbo].[User]
        ([UserId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Room_To_User'
CREATE INDEX [IX_FK_Room_To_User]
ON [dbo].[Room]
    ([MasterUserId]);
GO

-- Creating foreign key on [Game_GameId] in table 'GameMessage'
ALTER TABLE [dbo].[GameMessage]
ADD CONSTRAINT [FK_GameMessage_Game]
    FOREIGN KEY ([Game_GameId])
    REFERENCES [dbo].[Game]
        ([GameId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Message_MessageId] in table 'GameMessage'
ALTER TABLE [dbo].[GameMessage]
ADD CONSTRAINT [FK_GameMessage_Message]
    FOREIGN KEY ([Message_MessageId])
    REFERENCES [dbo].[Message]
        ([MessageId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_GameMessage_Message'
CREATE INDEX [IX_FK_GameMessage_Message]
ON [dbo].[GameMessage]
    ([Message_MessageId]);
GO

-- Creating foreign key on [Message_MessageId] in table 'RoomMessage'
ALTER TABLE [dbo].[RoomMessage]
ADD CONSTRAINT [FK_RoomMessage_Message]
    FOREIGN KEY ([Message_MessageId])
    REFERENCES [dbo].[Message]
        ([MessageId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Room_RoomId] in table 'RoomMessage'
ALTER TABLE [dbo].[RoomMessage]
ADD CONSTRAINT [FK_RoomMessage_Room]
    FOREIGN KEY ([Room_RoomId])
    REFERENCES [dbo].[Room]
        ([RoomId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RoomMessage_Room'
CREATE INDEX [IX_FK_RoomMessage_Room]
ON [dbo].[RoomMessage]
    ([Room_RoomId]);
GO

-- Creating foreign key on [GameId] in table 'GameEvent'
ALTER TABLE [dbo].[GameEvent]
ADD CONSTRAINT [FK_GameEvent_To_Game]
    FOREIGN KEY ([GameId])
    REFERENCES [dbo].[Game]
        ([GameId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_GameEvent_To_Game'
CREATE INDEX [IX_FK_GameEvent_To_Game]
ON [dbo].[GameEvent]
    ([GameId]);
GO

-- Creating foreign key on [NewPlayerId] in table 'GameEvent_PlayerChangeEvent'
ALTER TABLE [dbo].[GameEvent_PlayerChangeEvent]
ADD CONSTRAINT [FK_PlayerChangeEvent_To_Player]
    FOREIGN KEY ([NewPlayerId])
    REFERENCES [dbo].[User]
        ([UserId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PlayerChangeEvent_To_Player'
CREATE INDEX [IX_FK_PlayerChangeEvent_To_Player]
ON [dbo].[GameEvent_PlayerChangeEvent]
    ([NewPlayerId]);
GO

-- Creating foreign key on [PlayerId] in table 'GameEvent_DrawCardEvent'
ALTER TABLE [dbo].[GameEvent_DrawCardEvent]
ADD CONSTRAINT [FK_DrawCardEvent_To_Player]
    FOREIGN KEY ([PlayerId])
    REFERENCES [dbo].[User]
        ([UserId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DrawCardEvent_To_Player'
CREATE INDEX [IX_FK_DrawCardEvent_To_Player]
ON [dbo].[GameEvent_DrawCardEvent]
    ([PlayerId]);
GO

-- Creating foreign key on [DrawCardEventId] in table 'GameEvent_PlayCardEvent'
ALTER TABLE [dbo].[GameEvent_PlayCardEvent]
ADD CONSTRAINT [FK_PlayCardEvent_To_DrawCardEvent]
    FOREIGN KEY ([DrawCardEventId])
    REFERENCES [dbo].[GameEvent_DrawCardEvent]
        ([GameEventId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PlayCardEvent_To_DrawCardEvent'
CREATE INDEX [IX_FK_PlayCardEvent_To_DrawCardEvent]
ON [dbo].[GameEvent_PlayCardEvent]
    ([DrawCardEventId]);
GO

-- Creating foreign key on [ReceiverUserId] in table 'PrivateMessage'
ALTER TABLE [dbo].[PrivateMessage]
ADD CONSTRAINT [FK_ReceiverUserId]
    FOREIGN KEY ([ReceiverUserId])
    REFERENCES [dbo].[User]
        ([UserId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ReceiverUserId'
CREATE INDEX [IX_FK_ReceiverUserId]
ON [dbo].[PrivateMessage]
    ([ReceiverUserId]);
GO

-- Creating foreign key on [SenderUserId] in table 'PrivateMessage'
ALTER TABLE [dbo].[PrivateMessage]
ADD CONSTRAINT [FK_SenderUserId_To_User]
    FOREIGN KEY ([SenderUserId])
    REFERENCES [dbo].[User]
        ([UserId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SenderUserId_To_User'
CREATE INDEX [IX_FK_SenderUserId_To_User]
ON [dbo].[PrivateMessage]
    ([SenderUserId]);
GO

-- Creating foreign key on [GameEventId] in table 'GameEvent_PlayerChangeEvent'
ALTER TABLE [dbo].[GameEvent_PlayerChangeEvent]
ADD CONSTRAINT [FK_PlayerChangeEvent_inherits_GameEvent]
    FOREIGN KEY ([GameEventId])
    REFERENCES [dbo].[GameEvent]
        ([GameEventId])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [GameEventId] in table 'GameEvent_DrawCardEvent'
ALTER TABLE [dbo].[GameEvent_DrawCardEvent]
ADD CONSTRAINT [FK_DrawCardEvent_inherits_GameEvent]
    FOREIGN KEY ([GameEventId])
    REFERENCES [dbo].[GameEvent]
        ([GameEventId])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [GameEventId] in table 'GameEvent_PlayCardEvent'
ALTER TABLE [dbo].[GameEvent_PlayCardEvent]
ADD CONSTRAINT [FK_PlayCardEvent_inherits_GameEvent]
    FOREIGN KEY ([GameEventId])
    REFERENCES [dbo].[GameEvent]
        ([GameEventId])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------