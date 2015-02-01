
# README #


### How do I get set up? ###


* Configuración de la base de datos: (Database configuration)

En MySql:

```
 CREATE TABLE `sms` (
  `idSMS` int(11) NOT NULL AUTO_INCREMENT,
  `Origen` varchar(45) NOT NULL,
  `timeStamp` datetime DEFAULT NULL,
  `Msg` varchar(45) NOT NULL,
  PRIMARY KEY (`idSMS`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8;
```

* Como correr una prueba (How to run tests)

Para simular un SMS entrante se debe enviar vía [Hercules](http://new.hwg.cz/files/download/sw/version/hercules_3-2-8.exe) el siguiente mensaje como HEX por medio del puerto serial: 

...

0D 0A 2B 43 4D 54 3A 20 22 33 31 31 33 30 34 37 34 30 30 22 2C 2C 22 31 34 2F 31 30 2F 31 39 2C 31 30 3A 30 33 3A 35 38 2D 32 30 22 0D 0A 48 65 6C 6C 6F 20 77 6F 72 6C 64 21 21 21 0D 0A 

...

Los puertos seriales se pueden simular 

### Contribution guidelines ###



### Who do I talk to? ###
=======
# SMS_Server

