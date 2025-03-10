USE TareasDB1

CREATE TABLE Usuarios (
id INT IDENTITY(1,1) PRIMARY KEY,
nombreUsuario VARCHAR(50) UNIQUE NOT NULL,
contrasena VARCHAR(MAX) NOT NULL,
correo VARCHAR(100) UNIQUE NULL,
fechaCreacion DATETIME DEFAULT GETDATE()
);

CREATE TABLE Categorias (
id INT IDENTITY(1,1) PRIMARY KEY,
nombre VARCHAR(100) UNIQUE NOT NULL,
descripcion TEXT NULL,
fechaCreacion DATETIME DEFAULT GETDATE()
);

CREATE TABLE Estados (
id INT IDENTITY(1,1) PRIMARY KEY,
nombreEstado VARCHAR(50) UNIQUE NOT NULL
);

CREATE TABLE Tareas (
id INT IDENTITY(1,1) PRIMARY KEY,
titulo VARCHAR(255) NOT NULL,
descripcion TEXT NOT NULL,
categoriaId INT NOT NULL,
usuarioId INT NOT NULL,
estadoId INT NOT NULL,
fechaCreacion DATETIME DEFAULT GETDATE(),
fechaVencimiento DATE NULL,
FOREIGN KEY (categoriaId) REFERENCES Categorias(id) ON DELETE CASCADE,
FOREIGN KEY (usuarioId) REFERENCES Usuarios(id) ON DELETE CASCADE,
FOREIGN KEY (estadoId) REFERENCES Estados(id) ON DELETE CASCADE
);

INSERT INTO Usuarios (nombreUsuario, contrasena)VALUES ('admin', 'admin123');
INSERT INTO Estados (nombreEstado) Values ('Pendiente'), ('En Progreso'), ('Completa');



SELECT * FROM Tareas;


