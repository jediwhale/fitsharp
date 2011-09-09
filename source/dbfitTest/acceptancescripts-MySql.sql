create database dbfit;

grant all privileges on dbfit.* to dftest@localhost identified by 'dftest';

grant all privileges on dbfit.* to dftest@127.0.0.1 identified by 'dftest';

grant all privileges on dbfit.* to dbfit_user@localhost identified by 'password';

grant all privileges on dbfit.* to dbfit_user@127.0.0.1 identified by 'password';

grant select on mysql.* to dbfit_user@localhost;

flush privileges;

use dbfit;

create table users(name varchar(50) unique, username varchar(50), userid int auto_increment primary key);

CREATE PROCEDURE ConcatenateStrings (IN firststring varchar(100), IN secondstring varchar(100), OUT concatenated varchar(200)) set concatenated = concat(firststring , concat( ' ' , secondstring ));

create procedure CalcLength(IN name varchar(100), OUT strlength int) set strlength =length(name);

CREATE FUNCTION ConcatenateF (firststring  VARCHAR(100), secondstring varchar(100)) RETURNS VARCHAR(200) RETURN CONCAT(firststring,' ',secondstring);

create procedure makeuser() insert into users (name,username) values ('user1','fromproc');

create procedure createuser(IN newname varchar(100), IN newusername varchar(100)) insert into users (name,username) values (newname, newusername);

create procedure Multiply(IN factor int, INOUT val int) set val =val*factor;

Create table Test_DBFit(name varchar(50), luckyNumber int) engine=InnoDB;
