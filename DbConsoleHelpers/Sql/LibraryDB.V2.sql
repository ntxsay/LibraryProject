----------------------------------------------------------------------------------------------------
-- SQLITE --
--Bibliotheque -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TLibrary";
--Cree la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TLibrary" (
	"Id" INTEGER NOT NULL UNIQUE,
    "Guid" TEXT NOT NULL UNIQUE,--Si le dossier ce cree
    "Name" TEXT NOT NULL UNIQUE,
    "Description" TEXT NULL,
    "DateAjout" TEXT NOT NULL,
    "DateEdition" TEXT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
);

----------------------------------------------------------------------------------------------------
-- Categorie -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TLibraryCategorie";
--Cree la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TLibraryCategorie" (
	"Id" INTEGER NOT NULL UNIQUE,
	"IdLibrary" INTEGER NOT NULL,
    "Name" TEXT NOT NULL,
    "Description" TEXT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdLibrary") REFERENCES "TLibrary"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
-- Sous-Categorie -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TLibrarySubCategorie";
--Cree la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TLibrarySubCategorie" (
	"Id" INTEGER NOT NULL UNIQUE,
	"IdCategorie" INTEGER NOT NULL,
    "Name" TEXT NOT NULL,
    "Description" TEXT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdCategorie") REFERENCES "TLibraryCategorie"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
-- Collection -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TCollection";
--Cree la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TCollection" (
	"Id" INTEGER NOT NULL UNIQUE,
    "IdLibrary" INTEGER NOT NULL,
    "CollectionType" INTEGER NOT NULL DEFAULT 0, -- Collection, liste de lecture
    "Name" TEXT NOT NULL UNIQUE,
    "Description" TEXT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdLibrary") REFERENCES "TLibrary"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
--Contact emprunteur -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TContact";
--Cree la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TContact" (
	"Id" INTEGER NOT NULL UNIQUE,
    "Type" INTEGER NOT NULL DEFAULT 0, --Client, Auteur, maison d'édition, Entreprise
    "Guid" TEXT NOT NULL UNIQUE,--Si le dossier ce cree
    "DateAjout" TEXT NOT NULL,
    "DateEdition" TEXT NULL,
    "SocietyName" TEXT NULL,
    "TitreCivilite" TEXT NULL,
    "NomNaissance" TEXT NULL,
    "NomUsage" TEXT NULL,
    "Prenom" TEXT NULL,
    "AutresPrenoms" TEXT NULL,
    "DateNaissance" TEXT NULL,
    "DateDeces" TEXT NULL,
    "LieuNaissance" TEXT NULL,
    "LieuDeces" TEXT NULL,
    "Nationality" TEXT  NULL,
    "AdressPostal" TEXT NULL,
    "Ville" TEXT NULL,
    "CodePostal" TEXT NULL,
    "MailAdress" TEXT NULL,
    "NoTelephone" TEXT NULL,
    "NoMobile" TEXT NULL,
    "Observation" TEXT NULL,
	"Biographie" TEXT NULL,
    PRIMARY KEY("Id" AUTOINCREMENT)
);

----------------------------------------------------------------------------------------------------
--Livres -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TBook";
--Cree la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TBook" (
	"Id" INTEGER NOT NULL UNIQUE,
    "Guid" TEXT NOT NULL UNIQUE,--Si le dossier ce cree
    "DateAjout" TEXT NOT NULL,
    "DateEdition" TEXT NULL,
    "MainTitle" TEXT NOT NULL,
    "CountOpening" INTEGER NOT NULL DEFAULT 0,
    "DateParution" TEXT NULL,
    "Resume" TEXT NULL,
    "Notes" TEXT NULL,
    "TypeClassification" INTEGER  NOT NULL DEFAULT 0,
    "ApartirDe" INTEGER  NOT NULL DEFAULT 0,
    "Jusqua" INTEGER  NOT NULL DEFAULT 0,
    "DeTelAge" INTEGER  NOT NULL DEFAULT 0,
    "ATelAge" INTEGER  NOT NULL DEFAULT 0,
	"Langue" TEXT NULL,
	"Pays" TEXT NULL,
    "PhysicalLocation" TEXT NULL,

    PRIMARY KEY("Id" AUTOINCREMENT)
);

----------------------------------------------------------------------------------------------------
-- ISBN Livres -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TBookIdentification";
--Cree la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TBookIdentification" (
	"Id" INTEGER NOT NULL UNIQUE,
    "ISBN" TEXT NULL,
    "ISBN10" TEXT NULL,
    "ISBN13" TEXT NULL,
    "ISSN" TEXT NULL,
    "ASIN" TEXT NULL, --Id Amazon
    "Cotation" TEXT NULL,
    "CodeBarre" TEXT NULL, -- voir le scan avec syncfusion si possible
	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("Id") REFERENCES "TBook"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
-- Format Livres -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TBookFormat";
--Cree la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TBookFormat" (
	"Id" INTEGER NOT NULL UNIQUE,
    "Format" TEXT NULL, -- Relie, broche, cartonne, poche, audio, ebook
    "NbOfPages" INTEGER NULL,
    "Largeur" REAL NULL,
    "Hauteur" REAL NULL,
    "Epaisseur" REAL NULL,
    "Weight" REAL NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("Id") REFERENCES "TBook"("Id") ON DELETE CASCADE
);
----------------------------------------------------------------------------------------------------
-- Titres Livres -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TBookOtherTitle";
--Cree la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TBookOtherTitle" (
	"Id" INTEGER NOT NULL UNIQUE,
	"IdBook" INTEGER NOT NULL,
    "Title" TEXT NOT NULL UNIQUE,
	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdBook") REFERENCES "TBook"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
--Exemplaires Livres -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TBookExemplary";
--Cree la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TBookExemplary" (
	"Id" INTEGER NOT NULL UNIQUE,
	"IdBook" INTEGER NOT NULL,
	"IdContactSource" INTEGER NULL,
    "NoGroup" TEXT NULL,
    "NoExemplary" INTEGER NOT NULL DEFAULT 0,
    --"Quantity" INTEGER NOT NULL DEFAULT 0,
    "DateAjout" TEXT NOT NULL,
    "DateEdition" TEXT NULL,
    "TypeAcquisition" TEXT NOT NULL, --Donné, Acheté, preté, Autre 
    "Price" REAL NULL,
	"DeviceName" TEXT NULL,
    "DateAcquisition" TEXT NULL, --Date emprunt, Achat, Donation
    "DateRemise" TEXT NULL, --Si emprunt
    "IsVisible" INTEGER NOT NULL DEFAULT 1,
    "Observations" TEXT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdContactSource") REFERENCES "TContact"("Id") ON DELETE CASCADE
    FOREIGN KEY("IdBook") REFERENCES "TBook"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
-- Connecteur Categorie/Livres -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TLibraryBookConnector";
--Cree la Table si n'existe pas
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
    FOREIGN KEY("IdBook") REFERENCES "TBook"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
-- Connecteur Auteur/Livres -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TBookAuthorConnector";
--Cree la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TBookAuthorConnector" (
	"Id" INTEGER NOT NULL UNIQUE,
	"IdBook" INTEGER NOT NULL,
	"IdAuthor" INTEGER NOT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdBook") REFERENCES "TBook"("Id") ON DELETE CASCADE
    FOREIGN KEY("IdAuthor") REFERENCES "TContact"("Id") ON DELETE CASCADE
);


----------------------------------------------------------------------------------------------------
-- Connecteur Editeur/Livres -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TBookEditeurConnector";
--Cree la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TBookEditeurConnector" (
	"Id" INTEGER NOT NULL UNIQUE,
	"IdEditeur" INTEGER NOT NULL,
	"IdBook" INTEGER NOT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdEditeur") REFERENCES "TContact"("Id") ON DELETE CASCADE
    FOREIGN KEY("IdBook") REFERENCES "TBook"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
-- Connecteur Collection/Livres -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TBookCollectionConnector";
--Cree la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TBookCollectionConnector" (
	"Id" INTEGER NOT NULL UNIQUE,
	"IdCollection" INTEGER NOT NULL,
	"IdLibrary" INTEGER NOT NULL,
    "IdBook" INTEGER NOT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdCollection") REFERENCES "TCollection"("Id") ON DELETE CASCADE
    FOREIGN KEY("IdBook") REFERENCES "TBook"("Id") ON DELETE CASCADE
    FOREIGN KEY("IdLibrary") REFERENCES "TLibrary"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
-- Livre Etat -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TBookEtat";
--Cree la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TBookEtat" (
	"Id" INTEGER NOT NULL UNIQUE,
	"IdBookExemplary" INTEGER NOT NULL,
    "DateAjout" TEXT NOT NULL,
    "TypeVerification" INTEGER NOT NULL, -- Entrée, avant pret, apres pret, sortie
	"Etat" TEXT NOT NULL,
	"Observations" TEXT NULL,
    PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdBookExemplary") REFERENCES "TBookExemplary"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
-- Pret Livre -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TBookPret";
--Cree la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TBookPret" (
	"Id" INTEGER NOT NULL UNIQUE,
	"IdBookExemplary" INTEGER NOT NULL,
	"IdContact" INTEGER NOT NULL,
	"IdEtatBefore" INTEGER NOT NULL,
	"IdEtatAfter" INTEGER NULL,
    "DatePret" TEXT NOT NULL,
    "DateRemise" TEXT NULL,
    "DateRemiseUser" TEXT NULL, --new
    "Observation" TEXT NULL,

	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdContact") REFERENCES "TContact"("Id") ON DELETE CASCADE
    FOREIGN KEY("IdEtatBefore") REFERENCES "TBookEtat"("Id") ON DELETE CASCADE
    FOREIGN KEY("IdEtatAfter") REFERENCES "TBookEtat"("Id") ON DELETE CASCADE
    FOREIGN KEY("IdBookExemplary") REFERENCES "TBookExemplary"("Id") ON DELETE CASCADE
);