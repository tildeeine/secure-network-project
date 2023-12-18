CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

ALTER DATABASE CHARACTER SET utf8mb4;

CREATE TABLE `Patients` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` longtext CHARACTER SET utf8mb4 NOT NULL,
    `NIC` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Sex` nvarchar(24) NOT NULL,
    `DateOfBirth` longtext CHARACTER SET utf8mb4 NOT NULL,
    `BloodType` longtext CHARACTER SET utf8mb4 NOT NULL,
    `KnownAllergies` longtext CHARACTER SET utf8mb4 NOT NULL,
    `PublicKey` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Patients` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Consultation` (
    `PatientId` int NOT NULL,
    `Id` int NOT NULL AUTO_INCREMENT,
    `Date` longtext CHARACTER SET utf8mb4 NOT NULL,
    `NIC` longtext CHARACTER SET utf8mb4 NOT NULL,
    `MedicalSpeciality` longtext CHARACTER SET utf8mb4 NOT NULL,
    `DoctorName` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Practice` longtext CHARACTER SET utf8mb4 NOT NULL,
    `TreatmentSummary` longtext CHARACTER SET utf8mb4 NOT NULL,
    `PhysicianSignature` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Consultation` PRIMARY KEY (`Id`, `PatientId`),
    CONSTRAINT `FK_Consultation_Patients_PatientId` FOREIGN KEY (`PatientId`) REFERENCES `Patients` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

INSERT INTO `Patients` (`Id`, `BloodType`, `DateOfBirth`, `KnownAllergies`, `NIC`, `Name`, `PublicKey`, `Sex`)
VALUES (-1, 'O-', '2001-07-21', '["Chocolate"]', '000000000', 'Bob', CONCAT('-----BEGIN PUBLIC KEY-----', CHAR(10), 'MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEArzlh7J0ffHNn+aq2dijR', CHAR(10), 'LXs4MAGopAZhguwlvlWvWQ0uhPkONoz/Znda4RgAUmPSMbkGdnTT8rr8/7Imlwon', CHAR(10), 'i0n42U0i+UVJg12TObu7pTnVj5xsdsSu865r1d3fdVOSMAkDD183PILI6xzLKjoe', CHAR(10), 'CZtUTlg5oL5khL+cx9ofX3ofShMoYqQKpCK1bzstLQS53r05oFHG9YlbUHnGqGyH', CHAR(10), '7J+kJgkIqjRHVV5Aoon8Lcd9zn85DP258QBgDyJNWA48ZE6k9XvnOtW587/SWRxW', CHAR(10), '6DrJfwxvykuAFKS2t4mM/eeAxTiMo0nLLVBuAJ4QYkBqFP9/tB6dtBqRE/gYlINq', CHAR(10), 'pwIDAQAB', CHAR(10), '-----END PUBLIC KEY-----'), 'Male');

INSERT INTO `Consultation` (`Id`, `PatientId`, `Date`, `DoctorName`, `MedicalSpeciality`, `NIC`, `PhysicianSignature`, `Practice`, `TreatmentSummary`)
VALUES (-2, -1, '2023-02-02', 'Bob', 'Orthopedic', '000000000', 'e2LsTkrtPjE44NMyJzO9znj6unWOVPNZWvuG6P4eEGQbnP5ZxysBSsqUQNfDaySwXdsW6+iYXVxYQK/ZNbesqaGz8Hrtd1X+bHxSKLdQV1Vkw2Jpvr/MVPnBgVUbHL81KqdNk/g7wTwZ+8LWTf5sOdJGHIVX+QmWu41P3jbw6D1Q2yXzCWfqhHyUtHu8kgqZmUYijfwmF3s4A5zcYQxxqjXRc8XIjjuMT+iEfcIUulBuW5jPH5M8Oav2Lwqx+BQkNSXFzfKCWyfZGtMxMNNwTMxouOu/JSLX/OvjB07UVHVyMvEdcDmnvrQQl6I/ZkRbqzSXgnSejUviueAGmXguvw==', 'Clinica', 'Pomade'),
(-1, -1, '2023-02-02', 'Charlie', 'Neuro', '000000001', 'LOxx3nCQU7oHRHeiPgQtqWxDiwoZbfOkhBvtyDuLxKx5PwQE/sm9xsaQwVCns55B43Sz0s7FYNx27feGFZThfDg0HJnYMxL7T56XP80GSWwZHeJWevE9QjJNmE6cIG+c11BjVytRnACbC+IUBgoa+NFzNk23w04lseZd/2GJGl9GON7qiUssYTvZY03zHiMHjp79GYS6BcVHCWBFBZMVNEOujVpWBPd4jOlFwlSxjtzSAlgqBolArAHDO1BdWUmUPHVsCKJ9FVRDno0qYn64OgL2kkIPupqZ9TQftM6hi80vZalJKK7xRyijc09lmZs/53DegXJ6wbntzJOIHmUPYQ==', 'Clinica', 'Pomade');

CREATE UNIQUE INDEX `IX_Patients_NIC` ON `Patients` (`NIC`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20231218163234_InitialCreate', '8.0.0');

COMMIT;

