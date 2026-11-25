USE EquipoZAACC
GO
IF OBJECT_ID(N'dbo.Usuario', N'U') IS NULL
BEGIN
    ;THROW 51000, N'No existe dbo.Usuario en la base actual. Seleccione la base de datos ERP correcta y ejecute esta migración después del esquema base.', 1;
END;

SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID('dbo.PasskeyUsuario','U') IS NULL
BEGIN
    CREATE TABLE dbo.PasskeyUsuario (
        id_credencial       varbinary(1024) NOT NULL,
        id_usuario          int NOT NULL,
        user_handle         varbinary(64) NOT NULL,
        clave_publica       varbinary(4096) NOT NULL,
        contador_firma      bigint NOT NULL CONSTRAINT DF_PasskeyUsuario_contador DEFAULT 0,
        aaguid              uniqueidentifier NULL,
        nombre              nvarchar(100) NOT NULL,
        creada_en           datetime2(3) NOT NULL CONSTRAINT DF_PasskeyUsuario_creada DEFAULT SYSUTCDATETIME(),
        ultimo_uso_en       datetime2(3) NULL,
        revocada_en         datetime2(3) NULL,
        CONSTRAINT PK_PasskeyUsuario PRIMARY KEY NONCLUSTERED (id_credencial),
        CONSTRAINT FK_PasskeyUsuario_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario(id_usuario),
        CONSTRAINT CK_PasskeyUsuario_credencial CHECK (DATALENGTH(id_credencial) BETWEEN 1 AND 1024),
        CONSTRAINT CK_PasskeyUsuario_handle CHECK (DATALENGTH(user_handle) BETWEEN 1 AND 64),
        CONSTRAINT CK_PasskeyUsuario_publica CHECK (DATALENGTH(clave_publica) BETWEEN 1 AND 4096),
        CONSTRAINT CK_PasskeyUsuario_contador CHECK (contador_firma >= 0)
    );
END;

IF EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id=OBJECT_ID(N'dbo.PasskeyUsuario') AND name=N'PK_PasskeyUsuario' AND type=1
)
BEGIN
    ALTER TABLE dbo.PasskeyUsuario DROP CONSTRAINT PK_PasskeyUsuario;
    ALTER TABLE dbo.PasskeyUsuario ADD CONSTRAINT PK_PasskeyUsuario PRIMARY KEY NONCLUSTERED (id_credencial);
END
ELSE IF NOT EXISTS (
    SELECT 1 FROM sys.key_constraints
    WHERE parent_object_id=OBJECT_ID(N'dbo.PasskeyUsuario') AND name=N'PK_PasskeyUsuario'
)
BEGIN
    ALTER TABLE dbo.PasskeyUsuario ADD CONSTRAINT PK_PasskeyUsuario PRIMARY KEY NONCLUSTERED (id_credencial);
END;

IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('dbo.PasskeyUsuario') AND name='UX_PasskeyUsuario_user_handle')
    DROP INDEX UX_PasskeyUsuario_user_handle ON dbo.PasskeyUsuario;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('dbo.PasskeyUsuario') AND name='IX_PasskeyUsuario_user_handle')
    CREATE INDEX IX_PasskeyUsuario_user_handle ON dbo.PasskeyUsuario(user_handle);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('dbo.PasskeyUsuario') AND name='IX_PasskeyUsuario_usuario_activo')
    CREATE INDEX IX_PasskeyUsuario_usuario_activo ON dbo.PasskeyUsuario(id_usuario,revocada_en);

IF OBJECT_ID('dbo.Auditoria','U') IS NOT NULL
EXEC(N'
CREATE OR ALTER TRIGGER dbo.trg_Bitacora_PasskeyUsuario ON dbo.PasskeyUsuario
AFTER INSERT,UPDATE AS
BEGIN
    SET NOCOUNT ON;
    INSERT dbo.Auditoria(id_usuario,modulo,accion,entidad,id_registro,descripcion,equipo)
    SELECT TRY_CONVERT(int,SESSION_CONTEXT(N''id_usuario'')),''SEGURIDAD'',
        CASE WHEN d.id_usuario IS NULL THEN ''CREAR'' ELSE ''REVOCAR'' END,
        ''PasskeyUsuario'',CONVERT(nvarchar(100),COALESCE(i.id_usuario,d.id_usuario)),
        CASE WHEN d.id_usuario IS NULL THEN N''Se registró un passkey'' ELSE N''Se revocó un passkey'' END,HOST_NAME()
    FROM inserted i
    LEFT JOIN deleted d ON d.id_credencial=i.id_credencial
    WHERE d.id_usuario IS NULL OR (d.revocada_en IS NULL AND i.revocada_en IS NOT NULL);
END;');

COMMIT;
GO
