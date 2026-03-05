-- =============================================================================
-- Caro / Go Online Game Server  –  MySQL Schema
-- Database : server_game
-- Encoding : utf8mb4 / utf8mb4_unicode_ci
-- =============================================================================

CREATE DATABASE IF NOT EXISTS `server_game`
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE `server_game`;

-- =============================================================================
-- 1.  Players
--     UID = client IP address (auto-assigned by server on first connect)
--     Friends = JSON array: [{"UID":"...", "Name":"..."}, ...]
-- =============================================================================
CREATE TABLE IF NOT EXISTS `Players` (
  `ID`        INT           NOT NULL AUTO_INCREMENT,
  `Fullname`  VARCHAR(100)  NOT NULL,
  `UID`       VARCHAR(50)   NOT NULL,          -- client IP address
  `Friends`   LONGTEXT      NOT NULL DEFAULT ('[]'),  -- JSON array
  `Score`     INT           NOT NULL DEFAULT 1000,
  `Wins`      INT           NOT NULL DEFAULT 0,
  `Losses`    INT           NOT NULL DEFAULT 0,
  `Draws`     INT           NOT NULL DEFAULT 0,
  `CreatedAt` DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastSeen`  DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP
                            ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `uq_players_uid`      (`UID`),
  KEY        `idx_players_score`   (`Score` DESC),
  KEY        `idx_players_name`    (`Fullname`)
) ENGINE = InnoDB
  DEFAULT CHARSET = utf8mb4
  COLLATE = utf8mb4_unicode_ci
  COMMENT = 'Registered / auto-created players';


-- =============================================================================
-- 2.  Chats
--     IsGroupChat = 1  → ReceiverUID is ignored (broadcast to group channel)
--     IsGroupChat = 0  → private DM between SenderUID and ReceiverUID
-- =============================================================================
CREATE TABLE IF NOT EXISTS `Chats` (
  `ID`          INT          NOT NULL AUTO_INCREMENT,
  `SenderUID`   VARCHAR(50)  NOT NULL,
  `ReceiverUID` VARCHAR(50)  NOT NULL DEFAULT '',    -- '' for group messages
  `Message`     TEXT         NOT NULL,
  `Time`        DATETIME(3)  NOT NULL,               -- millisecond precision
  `IsGroupChat` TINYINT(1)   NOT NULL DEFAULT 0,
  PRIMARY KEY (`ID`),
  KEY `idx_chats_sender`       (`SenderUID`),
  KEY `idx_chats_receiver`     (`ReceiverUID`),
  KEY `idx_chats_time`         (`Time`),
  KEY `idx_chats_group_time`   (`IsGroupChat`, `Time`)  -- covers group-chat queries
) ENGINE = InnoDB
  DEFAULT CHARSET = utf8mb4
  COLLATE = utf8mb4_unicode_ci
  COMMENT = 'Private and group-channel chat messages';


-- =============================================================================
-- 3.  GameHistory
--     One row per completed (or in-progress) online match.
--     GameType: 0 = Caro 15x15,  1 = Go 19x19,  2 = 2v2 Caro 15x15
--     WinnerUID: NULL = in-progress, 'draw' = draw, otherwise = winning UID
-- =============================================================================
CREATE TABLE IF NOT EXISTS `GameHistory` (
  `ID`            INT          NOT NULL AUTO_INCREMENT,
  `RoomID`        VARCHAR(50)  NOT NULL,
  `GameType`      TINYINT(1)   NOT NULL DEFAULT 0,
  `Player1UID`    VARCHAR(50)  NOT NULL,
  `Player2UID`    VARCHAR(50)  NOT NULL DEFAULT '',  -- '' while waiting in 1v1
  `Team1AllyUID`  VARCHAR(50)  DEFAULT NULL,         -- 2v2 only
  `Team2AllyUID`  VARCHAR(50)  DEFAULT NULL,         -- 2v2 only
  `WinnerUID`     VARCHAR(50)  DEFAULT NULL,
  `MoveCount`     INT          NOT NULL DEFAULT 0,
  `StartTime`     DATETIME     NOT NULL,
  `EndTime`       DATETIME     DEFAULT NULL,
  `BoardSnapshot` LONGTEXT     DEFAULT NULL,         -- JSON of final board array
  PRIMARY KEY (`ID`),
  KEY `idx_gh_room`     (`RoomID`),
  KEY `idx_gh_player1`  (`Player1UID`),
  KEY `idx_gh_player2`  (`Player2UID`),
  KEY `idx_gh_start`    (`StartTime` DESC)
) ENGINE = InnoDB
  DEFAULT CHARSET = utf8mb4
  COLLATE = utf8mb4_unicode_ci
  COMMENT = 'History of every online game';


-- =============================================================================
-- 4.  Useful views  (optional – read-only, makes reporting easier)
-- =============================================================================

-- Leaderboard: top players by score
CREATE OR REPLACE VIEW `vw_leaderboard` AS
  SELECT ID, Fullname, UID, Score, Wins, Losses, Draws
  FROM   Players
  ORDER  BY Score DESC, Wins DESC;

-- Recent completed games (last 100)
CREATE OR REPLACE VIEW `vw_recent_games` AS
  SELECT gh.*, p1.Fullname AS Player1Name, p2.Fullname AS Player2Name
  FROM   GameHistory  gh
  LEFT   JOIN Players p1 ON p1.UID = gh.Player1UID
  LEFT   JOIN Players p2 ON p2.UID = gh.Player2UID
  WHERE  gh.WinnerUID IS NOT NULL
  ORDER  BY gh.EndTime DESC
  LIMIT  100;
