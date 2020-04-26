-- phpMyAdmin SQL Dump
-- version 4.8.5
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1:3306
-- Generation Time: Apr 26, 2020 at 03:41 AM
-- Server version: 5.7.26
-- PHP Version: 7.2.18

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `pursuit`
--

-- --------------------------------------------------------

--
-- Table structure for table `accounts`
--

DROP TABLE IF EXISTS `accounts`;
CREATE TABLE IF NOT EXISTS `accounts` (
  `name` varchar(24) NOT NULL,
  `uid` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `adminLevel` tinyint(3) UNSIGNED NOT NULL,
  `score` int(11) NOT NULL,
  `password` varchar(64) NOT NULL,
  `banExpireDate` bigint(20) NOT NULL DEFAULT '0',
  `banReason` varchar(128) NOT NULL DEFAULT '',
  `adminName` varchar(24) NOT NULL DEFAULT '',
  PRIMARY KEY (`uid`)
) ENGINE=MyISAM AUTO_INCREMENT=4 DEFAULT CHARSET=latin1;

--
-- Dumping data for table `accounts`
--

INSERT INTO `accounts` (`name`, `uid`, `adminLevel`, `score`, `password`, `banExpireDate`, `banReason`, `adminName`) VALUES
('Hype', 1, 3, 0, 'EF797C8118F02DFB649607DD5D3F8C7623048C9C063D532CC95C5ED7A898A64F', 0, 'test', 'Hype'),
('Hype2', 2, 0, 0, 'EF797C8118F02DFB649607DD5D3F8C7623048C9C063D532CC95C5ED7A898A64F', 0, ' test', 'Hype'),
('Ferdinando_Arieta', 3, 1, 0, '742A70D026931EC7D5D0E407E2C6AE07184011D1D16F12C2E9C676BDEA0950BE', 0, '', '');

-- --------------------------------------------------------

--
-- Table structure for table `reports`
--

DROP TABLE IF EXISTS `reports`;
CREATE TABLE IF NOT EXISTS `reports` (
  `reportedName` varchar(24) NOT NULL,
  `reporterName` varchar(24) NOT NULL,
  `date` bigint(20) NOT NULL,
  `reason` varchar(128) NOT NULL DEFAULT '',
  `uid` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`uid`)
) ENGINE=MyISAM AUTO_INCREMENT=9 DEFAULT CHARSET=latin1;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
