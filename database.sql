CREATE DATABASE IF NOT EXISTS socialfood;
USE socialfood;

CREATE TABLE users (
  `ID` varchar(40) NOT NULL,
  `User` varchar(20) NOT NULL,
  `Password` varchar(256) NOT NULL,
  `Firstname` varchar(30) NOT NULL,
  `Lastname` varchar(30) NOT NULL,
  PRIMARY KEY(`ID`)
) ENGINE=InnoDB;

CREATE TABLE images (
  `ID` varchar(40) NOT NULL,
  `IDUser` varchar(40) NOT NULL,
  `Path` varchar(255) NOT NULL,
  `Length` int NOT NULL,
  `Ora` datetime NOT NULL,
  `Descrizione` varchar(255) NOT NULL,
  `Luogo` varchar(255) NOT NULL,
  PRIMARY KEY(`ID`),
  FOREIGN KEY (`IDUser`) REFERENCES `users` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB;

CREATE TABLE friendships (
  `IDUserA` varchar(40) NOT NULL,
  `IDUserB` varchar(40) NOT NULL,
  PRIMARY KEY(`IDUserA`, `IDUserB`),
  FOREIGN KEY (`IDUserA`) REFERENCES `users` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY (`IDUserB`) REFERENCES `users` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB;

CREATE TABLE likes (
  `IDUser` varchar(40) NOT NULL,
  `IDImage` varchar(40) NOT NULL,
  PRIMARY KEY(`IDUser`, `IDImage`),
  FOREIGN KEY (`IDUser`) REFERENCES `users` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY (`IDImage`) REFERENCES `images` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB;
