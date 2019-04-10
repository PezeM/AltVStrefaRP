CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(95) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
);

CREATE TABLE `Characters` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `AccountId` int NOT NULL,
    `FirstName` longtext NULL,
    `LastName` longtext NULL,
    `Age` int NOT NULL,
    `Money` float NOT NULL,
    CONSTRAINT `PK_Characters` PRIMARY KEY (`Id`)
);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20190320132810_InitialMigration', '2.2.3-servicing-35854');

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20190320150312_AccountModel', '2.2.3-servicing-35854');

CREATE TABLE `Accounts` (
    `AccountId` int NOT NULL AUTO_INCREMENT,
    `Password` longtext NULL,
    `LicenseHash` longtext NULL,
    CONSTRAINT `PK_Accounts` PRIMARY KEY (`AccountId`)
);

CREATE INDEX `IX_Characters_AccountId` ON `Characters` (`AccountId`);

ALTER TABLE `Characters` ADD CONSTRAINT `FK_Characters_Accounts_AccountId` FOREIGN KEY (`AccountId`) REFERENCES `Accounts` (`AccountId`) ON DELETE CASCADE;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20190320150653_AccountRelations', '2.2.3-servicing-35854');

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20190320150908_AccountRelations2', '2.2.3-servicing-35854');

ALTER TABLE `Accounts` CHANGE `LicenseHash` `Username` longtext NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20190322151819_AddedAccountUsername', '2.2.3-servicing-35854');

ALTER TABLE `Characters` ADD `BackgroundImage` longtext NULL;

ALTER TABLE `Characters` ADD `ProfileImage` longtext NULL;

ALTER TABLE `Characters` ADD `TimePlayed` int NOT NULL DEFAULT 0;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20190323191628_CharacterImages', '2.2.3-servicing-35854');

ALTER TABLE `Characters` ADD `Dimension` smallint NOT NULL DEFAULT 0;

ALTER TABLE `Characters` ADD `Gender` int NOT NULL DEFAULT 0;

ALTER TABLE `Characters` ADD `X` float NOT NULL DEFAULT 0;

ALTER TABLE `Characters` ADD `Y` float NOT NULL DEFAULT 0;

ALTER TABLE `Characters` ADD `Z` float NOT NULL DEFAULT 0;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20190329184248_IgnorePlayerPropertyOnCharacter', '2.2.3-servicing-35854');

CREATE TABLE `Vehicles` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Owner` int NOT NULL,
    `X` float NOT NULL,
    `Y` float NOT NULL,
    `Z` float NOT NULL,
    `Dimension` smallint NOT NULL,
    `Fuel` float NOT NULL,
    `Oil` float NOT NULL,
    `IsJobVehicle` bit NOT NULL,
    CONSTRAINT `PK_Vehicles` PRIMARY KEY (`Id`)
);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20190401202013_AddedVehicleModel', '2.2.3-servicing-35854');

ALTER TABLE `Vehicles` ADD `IsSpawned` bit NOT NULL DEFAULT FALSE;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20190401203517_VehicleIsSpawned', '2.2.3-servicing-35854');

ALTER TABLE `Vehicles` ADD `Heading` float NOT NULL DEFAULT 0;

ALTER TABLE `Vehicles` ADD `IsBlocked` bit NOT NULL DEFAULT FALSE;

ALTER TABLE `Vehicles` ADD `IsLocked` bit NOT NULL DEFAULT FALSE;

ALTER TABLE `Vehicles` ADD `MaxFuel` float NOT NULL DEFAULT 0;

ALTER TABLE `Vehicles` ADD `MaxOil` float NOT NULL DEFAULT 0;

ALTER TABLE `Vehicles` ADD `Mileage` float NOT NULL DEFAULT 0;

ALTER TABLE `Vehicles` ADD `Model` longtext NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20190402120727_VehiclePropertiesUpdate', '2.2.3-servicing-35854');

ALTER TABLE `Characters` ADD `BankAccountId` int NOT NULL DEFAULT 0;

CREATE TABLE `BankAccounts` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Money` float NOT NULL,
    `AccountNumber` int NOT NULL,
    CONSTRAINT `PK_BankAccounts` PRIMARY KEY (`Id`)
);

CREATE TABLE `MoneyTransactions` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Source` int NOT NULL,
    `Receiver` int NOT NULL,
    `Date` longtext NULL,
    `Type` int NOT NULL,
    `Amount` float NOT NULL,
    CONSTRAINT `PK_MoneyTransactions` PRIMARY KEY (`Id`)
);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20190405083823_BankAccountModel', '2.2.3-servicing-35854');

ALTER TABLE `BankAccounts` ADD `CharacterId` int NOT NULL DEFAULT 0;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20190405092955_CharacterBankAccountId', '2.2.3-servicing-35854');

ALTER TABLE `Characters` DROP COLUMN `BankAccountId`;

CREATE UNIQUE INDEX `IX_BankAccounts_CharacterId` ON `BankAccounts` (`CharacterId`);

ALTER TABLE `BankAccounts` ADD CONSTRAINT `FK_BankAccounts_Characters_CharacterId` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20190405133916_BankAccountRelations', '2.2.3-servicing-35854');

ALTER TABLE `Characters` ADD `CreationDate` datetime(6) NOT NULL DEFAULT '0001-01-01 00:00:00.000000';

ALTER TABLE `Characters` ADD `LastPlayed` datetime(6) NOT NULL DEFAULT '0001-01-01 00:00:00.000000';

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20190407125816_AddedTimePlayedForCharacters', '2.2.3-servicing-35854');

ALTER TABLE `MoneyTransactions` MODIFY COLUMN `Source` longtext NULL;

ALTER TABLE `MoneyTransactions` MODIFY COLUMN `Receiver` longtext NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20190408195042_ChangedTypeOfMoneyTransactions', '2.2.3-servicing-35854');

ALTER TABLE `Vehicles` DROP COLUMN `IsJobVehicle`;

ALTER TABLE `Vehicles` ADD `OwnerType` int NOT NULL DEFAULT 0;

ALTER TABLE `Vehicles` ADD `PlateNumber` int NOT NULL DEFAULT 0;

ALTER TABLE `Vehicles` ADD `PlateText` longtext NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20190408200353_AddedVehicleOwnerTypeAndPlateNumberProperty', '2.2.3-servicing-35854');

ALTER TABLE `Vehicles` MODIFY COLUMN `PlateNumber` int unsigned NOT NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20190409153516_VehicleModelChanges', '2.2.3-servicing-35854');

CREATE TABLE `Businesses` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `OwnerId` int NOT NULL,
    `Title` longtext NULL,
    `X` float NOT NULL,
    `Y` float NOT NULL,
    `Z` float NOT NULL,
    `Money` float NOT NULL,
    CONSTRAINT `PK_Businesses` PRIMARY KEY (`Id`)
);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20190409212011_AddedBusinessModel', '2.2.3-servicing-35854');

