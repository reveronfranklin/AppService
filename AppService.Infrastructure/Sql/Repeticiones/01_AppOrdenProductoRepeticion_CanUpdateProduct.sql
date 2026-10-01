USE [RRD];
GO

IF OBJECT_ID(N'[dbo].[sp_AppOrdenProductoRepeticion_CanUpdateProduct]', N'P') IS NULL
BEGIN
    EXEC(N'CREATE PROCEDURE [dbo].[sp_AppOrdenProductoRepeticion_CanUpdateProduct]
        @UsuarioConectado NVARCHAR(50),
        @PuedeModificar BIT OUTPUT,
        @Message NVARCHAR(4000) OUTPUT
    AS
    BEGIN
        SET NOCOUNT ON;
    END');
END;
GO

ALTER PROCEDURE [dbo].[sp_AppOrdenProductoRepeticion_CanUpdateProduct]
    @UsuarioConectado NVARCHAR(50),
    @PuedeModificar BIT OUTPUT,
    @Message NVARCHAR(4000) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SET @UsuarioConectado = UPPER(LTRIM(RTRIM(ISNULL(@UsuarioConectado, N''))));
        SET @PuedeModificar = 0;

        IF @UsuarioConectado = N''
        BEGIN
            SET @Message = N'Usuario conectado requerido';
            RETURN;
        END;

        IF EXISTS
        (
            SELECT 1
            FROM dbo.MtrVendedor
            WHERE UPPER(LTRIM(RTRIM(Codigo))) = @UsuarioConectado
              AND Activo = 'X'
        )
        BEGIN
            SET @Message = N'Success';
            RETURN;
        END;

        SET @PuedeModificar = 1;
        SET @Message = N'Success';
    END TRY
    BEGIN CATCH
        SET @PuedeModificar = 0;
        SET @Message = ERROR_MESSAGE();
    END CATCH;
END;
GO
