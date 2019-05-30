EXEC sp_configure filestream_access_level, 2
RECONFIGURE

--------------------------------------------------------

CREATE DATABASE DB_A49398_HAmazonianWillow

--------------------------------------------------------

ALTER DATABASE DB_A49398_HAmazonianWillow
ADD FILEGROUP FILESTREAM_Data CONTAINS FILESTREAM 
GO

--------------------------------------------------------

ALTER DATABASE DB_A49398_HAmazonianWillow
ADD FILE
(
NAME = 'FILESTREAM_Data',
FILENAME = 'H:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\FILESTREAM_Data'
)
TO FILEGROUP FILESTREAM_Data
GO

--------------------------------------------------------

USE DB_A49398_HAmazonianWillow

--------------------------------------------------------
CREATE TABLE Estado(
	id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	descripcion VARCHAR(MAX) NOT NULL,
);

CREATE TABLE Imagen(
	id_Imagen INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	nombre VARCHAR(100) NOT NULL,
	id_Imagen_FileStream UNIQUEIDENTIFIER ROWGUIDCOL NOT NULL UNIQUE DEFAULT NEWID(),
	imagen VARBINARY(MAX) FILESTREAM NOT NULL
);

CREATE TABLE Tipo_Habitacion(
	id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	titulo VARCHAR(100) NOT NULL,
	tarifa float NOT NULL,
	descripcion VARCHAR(MAX) NOT NULL,
	imagen INT NOT NULL FOREIGN KEY REFERENCES Imagen(id_Imagen)
);

CREATE TABLE Habitacion(
	id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	numero INT NOT NULL,
	estado INT NOT NULL FOREIGN KEY REFERENCES Estado(id),
	tipo INT NOT NULL FOREIGN KEY REFERENCES Tipo_Habitacion(id)
);

CREATE TABLE Caracteristica(
	id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	descripcion VARCHAR(MAX) NOT NULL,
	tipo INT NOT NULL FOREIGN KEY REFERENCES Tipo_Habitacion(id)
);

CREATE TABLE Reservacion(
	id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	identificacion varchar(25),
	nombre VARCHAR(100) NOT NULL,
	apellidos VARCHAR(100) NOT NULL,
	correo VARCHAR(256) NOT NULL,
	tarjeta VARCHAR(256) NOT NULL,
	fechaVencimiento VARCHAR(256) NOT NULL,
	codigoSeguridad VARCHAR(256) NOT NULL,
	fechaInicio DATE NOT NULL,
	fechaFin DATE NOT NULL,
	monto FLOAT NOT NULL,
	habitacion INT NOT NULL FOREIGN KEY REFERENCES Habitacion (id),
	estado INT NOT NULL FOREIGN KEY REFERENCES Estado(id)
);

CREATE TABLE Publicidad(
	id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	url VARCHAR(1000) NOT NULL,
	imagen INT NOT NULL FOREIGN KEY REFERENCES Imagen(id_Imagen),
	estado INT NOT NULL FOREIGN KEY REFERENCES Estado(id)
);

CREATE TABLE Promocion(
	id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	descripcion varchar (100) Not Null,
	descuento int,
	inicio DATE,
	fin DATE,
	tipoHabitacion INT NOT NULL FOREIGN KEY REFERENCES Tipo_Habitacion(id)
);

CREATE TABLE Pagina(
	id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	nombre VARCHAR(100) NOT NULL
);

CREATE TABLE Info(
	id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	descripcion VARCHAR(MAX),
	titulo VARCHAR(MAX),
	idPagina INT NOT NULL FOREIGN KEY REFERENCES Pagina (id),
	imagen INT FOREIGN KEY REFERENCES Imagen(id_Imagen)
);


CREATE TABLE Administrador(
	id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	correo VARCHAR(100) NOT NULL UNIQUE,
	contrasenna VARCHAR(100) NOT NULL,
	nombre Varchar(25) NOT NULL
);

--------------------------------------------------------

exec sp_checkAvailability 7, '2019-06-01', '2019-07-02'

CREATE PROCEDURE sp_checkAvailability @idTipoHabitacion INT, @fechaInicio Date, @fechaFin Date
AS BEGIN
	BEGIN TRANSACTION
	BEGIN TRY
	Declare @numero int, @titulo varchar(100), @descripcion varchar(max), @tarifa float, @imagen int
		If exists(SELECT Habitacion.numero FROM Habitacion WHERE Habitacion.tipo = @idTipoHabitacion AND Habitacion.estado = 1)
		BEGIN
			SELECT TOP 1 @numero = h.numero, @titulo = th.titulo, @descripcion = th.descripcion, @tarifa = th.tarifa, @imagen = i.id_Imagen
			FROM Habitacion h
			Join Tipo_Habitacion th on h.tipo = th.id Join Imagen i on th.imagen = i.id_Imagen
			WHERE th.id = @idTipoHabitacion AND h.estado = 1;

			Update Habitacion Set Habitacion.estado = 8 Where Habitacion.numero = @numero;

			SELECT @numero as numero, @titulo as titulo, @descripcion as descripcion, @tarifa as tarifa, @imagen as imagen;
		END
		Else
		Begin
			Set @numero = 0;
			Set @titulo = '';
			Set @descripcion = 'No hay Habitaciones';
			Set @tarifa = 0;
			Set @imagen = 0;
			SELECT @numero as numero, @titulo as titulo, @descripcion as descripcion, @tarifa as tarifa, @imagen as imagen;
		End
	COMMIT TRANSACTION;
	RETURN (1);
	END TRY  
	BEGIN CATCH  
		ROLLBACK		
		SELECT   
        ERROR_NUMBER() AS ErrorNumber, ERROR_SEVERITY() AS ErrorSeverity,
        ERROR_STATE() AS ErrorState, ERROR_PROCEDURE() AS ErrorProcedure,  
        ERROR_LINE() AS ErrorLine, ERROR_MESSAGE() AS ErrorMessage;  
		RETURN(-1)
	END CATCH;  		
END
GO

CREATE PROCEDURE sp_makeReservation @identificacion VARCHAR(25), @nombre VARCHAR(100), @apellidos VARCHAR(100), @correo VARCHAR(256), 
	@tarjeta VARCHAR(256), @fechaVencimiento VARCHAR(256), @codigoSeguridad VARCHAR(256), @numero INT, @fechaLlegada DATETIME, @fechaSalida DATETIME, @monto FLOAT
									 
AS BEGIN
	BEGIN TRANSACTION
	BEGIN TRY  
	Declare @numeroReserva int, @idHabitacion int

	Select @idHabitacion = id from Habitacion where Numero = @numero;

	INSERT INTO Reservacion (identificacion, nombre, apellidos, correo, tarjeta, fechaVencimiento, codigoSeguridad, fechaInicio, fechaFin, monto, habitacion, estado)
	VALUES(@identificacion, @nombre, @apellidos, @correo, @tarjeta, @fechaVencimiento, @codigoSeguridad, @fechaLlegada, @fechaSalida, @monto,@numero, 7);	
	
	Select @numeroReserva = id From Reservacion where habitacion = @numero AND identificacion = @identificacion AND fechaInicio = @fechaLlegada AND fechaFin = @fechaSalida;

	Update Habitacion Set Habitacion.estado = 7 where Habitacion.id = @numero;

	SELECT @nombre + ' ' + @apellidos as nombre, @numeroReserva as numeroReserva, @correo as correo, @monto as monto;

	COMMIT TRANSACTION;
	RETURN (1);
	END TRY  
	BEGIN CATCH  
		rollback		
		SELECT   
        ERROR_NUMBER() AS ErrorNumber, ERROR_SEVERITY() AS ErrorSeverity  
        ,ERROR_STATE() AS ErrorState, ERROR_PROCEDURE() AS ErrorProcedure  
        ,ERROR_LINE() AS ErrorLine, ERROR_MESSAGE() AS ErrorMessage;  
		return(-1)
		
	END CATCH;  		
END

GO

CREATE PROCEDURE sp_freeRoom @numero INT
									 
AS BEGIN
	BEGIN TRANSACTION
	BEGIN TRY  
	
	Update Habitacion Set Habitacion.estado = 1 where Habitacion.Numero = @numero;

	COMMIT TRANSACTION;
	RETURN (1);
	END TRY  
	BEGIN CATCH  
		rollback		
		SELECT   
        ERROR_NUMBER() AS ErrorNumber, ERROR_SEVERITY() AS ErrorSeverity  
        ,ERROR_STATE() AS ErrorState, ERROR_PROCEDURE() AS ErrorProcedure  
        ,ERROR_LINE() AS ErrorLine, ERROR_MESSAGE() AS ErrorMessage;  
		return(-1)
		
	END CATCH;  		
END
GO

Create procedure [dbo].[sp_insertImage] @nombre VARCHAR(100), @imagen varbinary(max)
AS BEGIN
	BEGIN TRANSACTION
	BEGIN TRY  
	
	Insert into Imagen(nombre, Imagen) Values(@nombre, @imagen)

	SELECT SCOPE_IDENTITY() AS id_Imagen

	COMMIT TRANSACTION;
	RETURN (1);
	END TRY  
	BEGIN CATCH  
		rollback		
		SELECT   
        ERROR_NUMBER() AS ErrorNumber, ERROR_SEVERITY() AS ErrorSeverity  
        ,ERROR_STATE() AS ErrorState, ERROR_PROCEDURE() AS ErrorProcedure  
        ,ERROR_LINE() AS ErrorLine, ERROR_MESSAGE() AS ErrorMessage;  
		return(-1)
		
	END CATCH;  		
END
GO

Create PROCEDURE sp_roomDay
as
begin

	select h.numero as numero, th.titulo as tipo, e.descripcion as estado, th.tarifa as tarifa 
		from	Habitacion	h										left join 
				Reservacion r		on r.habitacion	=	h.id		inner join 
				Estado e			on e.id			=	h.estado	inner join
				Tipo_Habitacion th	on th.id		=	h.tipo
end

Create PROCEDURE sp_getFacilities
as
begin

	select  titulo as titulo ,descripcion as descripcion,img.imagen as imagen
		from info i inner join Imagen img on img.id_Imagen = i.imagen where i.idPagina=4
end

Create PROCEDURE sp_AvailabilityDay
as
begin
	select h.id as idHabitacion,numero,h.estado,tipo,nombre,fechaInicio,fechaFin from Habitacion h inner join Reservacion r on r.habitacion=h.id
	where fechaInicio <=(CONVERT(VARCHAR(10), GETDATE(), 23)) and (CONVERT(VARCHAR(10), GETDATE(), 23))<=fechaFin
end

Create procedure sp_check_Rooms_Available @llegada datetime, @salida datetime, @tipo int
as
begin
	select 
	Habitacion.numero as habitacion, 
	Tipo_Habitacion.titulo as tipo, 
	Tipo_Habitacion.tarifa

	from Habitacion, Tipo_Habitacion

	where 
		Habitacion.tipo = Tipo_Habitacion.id and
		(Habitacion.tipo = @tipo or @tipo = 0) and -- Si es 0 envia todos los tipos
		Habitacion.id not in ( 
			select Reservacion.habitacion 
			from Reservacion, Habitacion 
			where (
				Habitacion.id=Reservacion.habitacion  and (
				(@llegada between Reservacion.fechaInicio and Reservacion.fechaFin) or
				(@salida between Reservacion.fechaInicio and Reservacion.fechaFin) or
				(Reservacion.fechaInicio between @llegada and @salida) or
				 (Reservacion.fechaFin between @llegada and @salida)
				)
			) -- where
		) --not in
end