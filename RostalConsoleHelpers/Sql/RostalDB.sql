----------------------------------------------------------------------------------------------------
--Bibliothèque -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TLibrary";
--Crée la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TLibrary" (
	"Id" INTEGER NOT NULL UNIQUE,
    "Guid" TEXT NOT NULL UNIQUE,--Si le dossier ce crée
    "Name" TEXT NOT NULL UNIQUE,
    "Description" TEXT NULL,
    "DateAjout" TEXT NOT NULL,
    "DateEdition" TEXT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
);

----------------------------------------------------------------------------------------------------
-- Catégorie -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TLibraryCategorie";
--Crée la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TLibraryCategorie" (
	"Id" INTEGER NOT NULL UNIQUE,
	"IdLibrary" INTEGER NOT NULL,
    "Name" TEXT NOT NULL,
    "Description" TEXT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdLibrary") REFERENCES "TLibrary"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
-- Sous-Catégorie -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TLibrarySubCategorie";
--Crée la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TLibrarySubCategorie" (
	"Id" INTEGER NOT NULL UNIQUE,
	"IdCategorie" INTEGER NOT NULL,
    "Name" TEXT NOT NULL,
    "Description" TEXT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdCategorie") REFERENCES "TLibraryCategorie"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
--Livres -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TBooks";
--Crée la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TBooks" (
	"Id" INTEGER NOT NULL UNIQUE,
    "Guid" TEXT NOT NULL UNIQUE,--Si le dossier ce crée
    "ISBN" TEXT NULL UNIQUE,
    "Cotation" TEXT NULL UNIQUE,
    "AnneeParution" INTEGER NULL,
    "Description" TEXT NULL,
    "DateAjout" TEXT NOT NULL,
    "DateEdition" TEXT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
);


----------------------------------------------------------------------------------------------------
-- Titres Livres -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TBookTitle";
--Crée la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TBookTitle" (
	"Id" INTEGER NOT NULL UNIQUE,
	"IdBook" INTEGER NOT NULL,
    "Name" TEXT NOT NULL UNIQUE,
	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdBook") REFERENCES "TBooks"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
-- Auteurs Livres -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TBookAuthor";
--Crée la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TBookAuthor" (
	"Id" INTEGER NOT NULL UNIQUE,
	"IdBook" INTEGER NOT NULL,
    "Name" TEXT NOT NULL UNIQUE,
	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdBook") REFERENCES "TBooks"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
-- Connecteur Catégorie/Livres -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TLibraryBookConnector";
--Crée la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TLibraryBookConnector" (
	"Id" INTEGER NOT NULL UNIQUE,
	"IdLibrary" INTEGER NOT NULL,
	"IdCategorie" INTEGER NULL,
	"IdSubCategorie" INTEGER NULL,
	"IdBook" INTEGER NOT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdLibrary") REFERENCES "TLibrary"("Id") ON DELETE CASCADE
    FOREIGN KEY("IdCategorie") REFERENCES "TLibraryCategorie"("Id") ON DELETE CASCADE
    FOREIGN KEY("IdSubCategorie") REFERENCES "TLibrarySubCategorie"("Id") ON DELETE CASCADE
    FOREIGN KEY("IdBook") REFERENCES "TBooks"("Id") ON DELETE CASCADE
);

