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

 Date: 03/05/2024 14:57:16
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for registeredmcos
-- ----------------------------
DROP TABLE IF EXISTS `registeredmcos`;
CREATE TABLE `registeredmcos`  (
  `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `stubno` int(11) NOT NULL,
  `mconame` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `district` int(255) NULL DEFAULT NULL,
  `flag_winner` tinyint(1) NULL DEFAULT NULL,
  `drawtime` datetime NULL DEFAULT NULL,
  `prize` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `claimed` tinyint(1) NULL DEFAULT 0,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of registeredmcos
-- ----------------------------

SET FOREIGN_KEY_CHECKS = 1;
