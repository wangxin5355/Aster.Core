use aster
DROP TABLE IF EXISTS t_user;
CREATE TABLE t_user  (
  `id` int(20) NOT NULL AUTO_INCREMENT COMMENT '用户id',
  `userName` varchar(50) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL COMMENT '用户名称',
  `passWord` varchar(50) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL COMMENT '密码',
  `phone` varchar(50) CHARACTER SET utf8 COLLATE utf8_bin NULL DEFAULT NULL COMMENT '手机号码',
  `email` varchar(50) CHARACTER SET utf8 COLLATE utf8_bin NULL DEFAULT NULL COMMENT '电子邮件',
  `createTime` datetime(0) NOT NULL COMMENT '创建时间',
  `lastUpdateTime` datetime(0) NULL DEFAULT NULL COMMENT '最后更新时间',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB  CHARACTER SET = utf8 ;

DROP TABLE IF EXISTS t_token;
CREATE TABLE t_token  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `userId` int(11) NOT NULL,
  `token` varchar(255) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL COMMENT 'token',
  `packType` varchar(10) CHARACTER SET utf8 COLLATE utf8_bin NULL DEFAULT NULL COMMENT '平台类型',
  `createTime` bigint(50) NOT NULL COMMENT '创建',
  `expireTime` bigint(20) NOT NULL COMMENT '到期时间',
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `idx_userId_pkType`(`userId`, `packType`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 ;

