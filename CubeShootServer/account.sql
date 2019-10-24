/*
Navicat MySQL Data Transfer

Source Server         : localhost_3306
Source Server Version : 50726
Source Host           : localhost:3306
Source Database       : darkgod

Target Server Type    : MYSQL
Target Server Version : 50726
File Encoding         : 65001

Date: 2019-08-06 20:27:29
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for `account`
-- ----------------------------
DROP TABLE IF EXISTS `account`;
CREATE TABLE `account` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `acct` varchar(255) NOT NULL,
  `pass` varchar(255) NOT NULL,
  `name` varchar(255) NOT NULL,
  `level` int(11) NOT NULL,
  `exp` int(11) NOT NULL,
  `power` int(11) NOT NULL,
  `coin` int(11) NOT NULL,
  `diamond` int(11) NOT NULL,
  `hp` int(11) NOT NULL,
  `ad` int(11) NOT NULL,
  `ap` int(11) NOT NULL,
  `addef` int(11) NOT NULL,
  `apdef` int(11) NOT NULL,
  `dodge` int(11) NOT NULL,
  `pierce` int(11) NOT NULL,
  `critical` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=15 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of account
-- ----------------------------
INSERT INTO `account` VALUES ('7', '12345', '1222', '吕君', '1', '0', '200', '1000', '0', '2000', '275', '265', '67', '43', '7', '5', '2');
INSERT INTO `account` VALUES ('8', 'wq', '1222', '楚融', '1', '0', '150', '1000', '0', '2000', '275', '265', '67', '43', '7', '5', '2');
INSERT INTO `account` VALUES ('9', 'ew', '1222', '乐文', '1', '0', '150', '1000', '0', '2000', '275', '265', '67', '43', '7', '5', '2');
INSERT INTO `account` VALUES ('10', 'eww', '1222', '祝卫', '1', '0', '150', '1000', '0', '2000', '275', '265', '67', '43', '7', '5', '2');
INSERT INTO `account` VALUES ('11', 'wqwq', '1222', '于霄', '1', '0', '150', '1000', '0', '2000', '275', '265', '67', '43', '7', '5', '2');
INSERT INTO `account` VALUES ('12', 'dada', '1222', '喻赋', '1', '0', '150', '1000', '0', '2000', '275', '265', '67', '43', '7', '5', '2');
INSERT INTO `account` VALUES ('13', 'wqwqwqw', '1222', '马墨', '1', '0', '150', '1000', '0', '2000', '275', '265', '67', '43', '7', '5', '2');
INSERT INTO `account` VALUES ('14', 'sa', '1222', '轩辕野', '1', '0', '150', '1000', '0', '2000', '275', '265', '67', '43', '7', '5', '2');
