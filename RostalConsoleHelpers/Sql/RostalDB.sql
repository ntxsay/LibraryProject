----------------------------------------------------------------------------------------------------
--Biblioth�que -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TLibrary";
--Cr�e la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TLibrary" (
	"Id" INTEGER NOT NULL UNIQUE,
    "Guid" TEXT NOT NULL UNIQUE,--Si le dossier ce cr�e
    "Name" TEXT NOT NULL UNIQUE,
    "Description" TEXT NULL,
    "DateAjout" TEXT NOT NULL,
    "DateEdition" TEXT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
);

----------------------------------------------------------------------------------------------------
-- Cat�gorie -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TLibraryCategorie";
--Cr�e la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TLibraryCategorie" (
	"Id" INTEGER NOT NULL UNIQUE,
	"IdLibrary" INTEGER NOT NULL,
    "Name" TEXT NOT NULL,
    "Description" TEXT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdLibrary") REFERENCES "TLibrary"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
-- Sous-Cat�gorie -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TLibrarySubCategorie";
--Cr�e la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TLibrarySubCategorie" (
	"Id" INTEGER NOT NULL UNIQUE,
	"IdCategorie" INTEGER NOT NULL,
    "Name" TEXT NOT NULL,
    "Description" TEXT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdCategorie") REFERENCES "TLibraryCategorie"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
--Contact emprunteur -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TContact";
--Cr�e la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TContact" (
	"Id" INTEGER NOT NULL UNIQUE,
    "Guid" TEXT NOT NULL UNIQUE,--Si le dossier ce cr�e
    "DateAjout" TEXT NOT NULL,
    "DateEdition" TEXT NULL,
    "NomNaissance" TEXT NOT NULL,
    "NomUsage" TEXT NULL,
    "Prenom" TEXT NOT NULL,
    "AutresPrenoms" TEXT NULL,
    "AdressPostal" TEXT NULL,
    "Ville" TEXT NULL,
    "CodePostal" TEXT NULL,
    "MailAdress" TEXT NULL,
    "NoTelephone" TEXT NULL,
    "NoMobile" TEXT NULL,
    "Observation" TEXT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
);

----------------------------------------------------------------------------------------------------
--Livres -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TBooks";
--Cr�e la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TBooks" (
	"Id" INTEGER NOT NULL UNIQUE,
    "Guid" TEXT NOT NULL UNIQUE,--Si le dossier ce cr�e
    "DateAjout" TEXT NOT NULL,
    "DateAjoutUser" TEXT NOT NULL,
    "DateEdition" TEXT NULL,
    "CountOpening" INTEGER NOT NULL DEFAULT 0,
    "ISBN" TEXT NULL,
    "ISSN" TEXT NULL,
    "Cotation" TEXT NULL,
    "NumberOfPages" INTEGER NULL,
    "NumberOfExemplaire" INTEGER NOT NULL DEFAULT 0,
    "AnneeParution" INTEGER NULL,
    "DateParution" TEXT NULL,
    "Description" TEXT NULL,
    "Notes" TEXT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
);


----------------------------------------------------------------------------------------------------
-- Titres Livres -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TBookTitle";
--Cr�e la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TBookTitle" (
	"Id" INTEGER NOT NULL UNIQUE,
	"IdBook" INTEGER NOT NULL,
    "Name" TEXT NOT NULL UNIQUE,
	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdBook") REFERENCES "TBooks"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
-- Langues Livres -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TBookLangue";
--Cr�e la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TBookLangue" (
	"Id" INTEGER NOT NULL UNIQUE,
	"IdBook" INTEGER NOT NULL,
    "Langue" TEXT NOT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdBook") REFERENCES "TBooks"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
-- Auteurs Livres -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TBookAuthor";
--Cr�e la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TBookAuthor" (
	"Id" INTEGER NOT NULL UNIQUE,
	"IdBook" INTEGER NOT NULL,
    "Name" TEXT NOT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdBook") REFERENCES "TBooks"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
-- Livre Prix -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TBookPrice";
--Cr�e la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TBookPrice" (
	"Id" INTEGER NOT NULL UNIQUE,
	"IdBook" INTEGER NOT NULL,
    "Price" REAL NOT NULL,
	"DeviceName" TEXT NOT NULL,
	"DeviceChar" TEXT NOT NULL,
    PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdBook") REFERENCES "TBooks"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
-- Livre Etat -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TBookEtat";
--Cr�e la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TBookEtat" (
	"Id" INTEGER NOT NULL UNIQUE,
	"IdBook" INTEGER NOT NULL,
    "DateAjout" TEXT NOT NULL,
	"Etat" TEXT NOT NULL,
	"Observation" TEXT NULL,
    PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdBook") REFERENCES "TBooks"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
-- Connecteur Cat�gorie/Livres -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TLibraryBookConnector";
--Cr�e la Table si n'existe pas
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

----------------------------------------------------------------------------------------------------
-- Editeurs Livres -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TBookEditeur";
--Cr�e la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TBookEditeur" (
	"Id" INTEGER NOT NULL UNIQUE,
    "Name" TEXT NOT NULL UNIQUE,
    "Adress" TEXT NOT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
);


----------------------------------------------------------------------------------------------------
-- Connecteur Editeur/Livres -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TEditorsBookConnector";
--Cr�e la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TEditorsBookConnector" (
	"Id" INTEGER NOT NULL UNIQUE,
	"IdEditeur" INTEGER NOT NULL,
	"IdBook" INTEGER NOT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdEditeur") REFERENCES "TBookEditeur"("Id") ON DELETE CASCADE
    FOREIGN KEY("IdBook") REFERENCES "TBooks"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
-- Pret Livre -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TBookPret";
--Cr�e la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TBookPret" (
	"Id" INTEGER NOT NULL UNIQUE,
	"IdBook" INTEGER NOT NULL,
	"IdContact" INTEGER NOT NULL,
	"IdEtatBefore" INTEGER NOT NULL,
	"IdEtatAfter" INTEGER NULL,
    "DatePret" TEXT NOT NULL,
    "DateRemise" TEXT NULL,
    "Observation" TEXT NULL,

	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdContact") REFERENCES "TContact"("Id") ON DELETE CASCADE
    FOREIGN KEY("IdEtatBefore") REFERENCES "TBookEtat"("Id") ON DELETE CASCADE
    FOREIGN KEY("IdEtatAfter") REFERENCES "TBookEtat"("Id") ON DELETE CASCADE
    FOREIGN KEY("IdBook") REFERENCES "TBooks"("Id") ON DELETE CASCADE
);


