/*
 Navicat Premium Data Transfer

 Source Server         : Localhost
 Source Server Type    : MariaDB
 Source Server Version : 100428
 Source Host           : localhost:3306
 Source Schema         : agmaraffle

 Target Server Type    : MariaDB
 Target Server Version : 100428
 File Encoding         : 65001

 Date: 03/05/2024 14:57:07
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for prizes
-- ----------------------------
DROP TABLE IF EXISTS `prizes`;
CREATE TABLE `prizes`  (
  `prizeid` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT,
  `prize_name` varchar(255) CHARACTER SET utf8 COLLATE utf8_bin NULL DEFAULT NULL,
  `total` int(255) NULL DEFAULT NULL,
  `issued` int(255) NULL DEFAULT NULL,
  `preferreddistrict` int(255) NULL DEFAULT NULL,
  `otherdist_count` int(255) NULL DEFAULT NULL,
  PRIMARY KEY (`prizeid`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 9 CHARACTER SET = utf8 COLLATE = utf8_bin ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of prizes
-- ----------------------------
INSERT INTO `prizes` VALUES (1, 'Hisense Smart TV', 10, 0, 8, 3);
INSERT INTO `prizes` VALUES (2, 'Android Phone', 10, 0, 8, 3);
INSERT INTO `prizes` VALUES (3, 'Microwave Oven', 10, 0, 8, 3);
INSERT INTO `prizes` VALUES (4, 'Electric Rice Cooker', 10, 0, 8, 3);
INSERT INTO `prizes` VALUES (5, 'AM/FM Radio', 10, 0, 8, 3);
INSERT INTO `prizes` VALUES (6, 'Oven Toaster', 10, 0, 8, 3);
INSERT INTO `prizes` VALUES (7, 'Stand Fan', 10, 0, 8, 3);
INSERT INTO `prizes` VALUES (8, 'Flat Iron', 10, 0, 8, 3);

SET FOREIGN_KEY_CHECKS = 1;
