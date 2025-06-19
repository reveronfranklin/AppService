CREATE PROCEDURE InsertAppCalculadora
    @IdVendedor NVARCHAR(50),
    @IdMunicipio INT,
    @DescripcionMunicipio NVARCHAR(255),
    @IdCondPago INT,
    @DescripcionCondPago NVARCHAR(255),
    @IdMtrTipoMonedaPago INT,
    @DescripcionTipoMonedaPAgo NVARCHAR(255),
    @IdSubcategoria INT,
    @DescripcionSubcategoria NVARCHAR(255),
    @IdProducto INT,
    @DescripcionProducto NVARCHAR(255),
    @Cantidad DECIMAL(18, 2),
    @IdUnidad INT,
    @DescripcionUnidad NVARCHAR(255),
    @Cantidad_X_Unidad DECIMAL(18, 2),
    @CantidadConvertida DECIMAL(18, 2),
    @MedidaBasica NVARCHAR(50),
    @MedidaOpuesta NVARCHAR(50),
    @PrecioMinimo DECIMAL(18, 2),
    @PrecioMaximo DECIMAL(18, 2),
    @Precio DECIMAL(18, 2),
    @Total DECIMAL(18, 2),
    @CalculoId INT = NULL,
    @UserCreate NVARCHAR(50),
    @SearchText NVARCHAR(MAX) = NULL
AS
BEGIN
INSERT INTO AppCalculadora (
    IdVendedor, IdMunicipio, DescripcionMunicipio, IdCondPago, DescripcionCondPago,
    IdMtrTipoMonedaPago, DescripcionTipoMonedaPAgo, IdSubcategoria, DescripcionSubcategoria,
    IdProducto, DescripcionProducto, Cantidad, IdUnidad, DescripcionUnidad, Cantidad_X_Unidad,
    CantidadConvertida, MedidaBasica, MedidaOpuesta, PrecioMinimo, PrecioMaximo, Precio, Total,
    CalculoId, UserCreate, CreatedAt, SearchText
)
VALUES (
           @IdVendedor, @IdMunicipio, @DescripcionMunicipio, @IdCondPago, @DescripcionCondPago,
           @IdMtrTipoMonedaPago, @DescripcionTipoMonedaPAgo, @IdSubcategoria, @DescripcionSubcategoria,
           @IdProducto, @DescripcionProducto, @Cantidad, @IdUnidad, @DescripcionUnidad, @Cantidad_X_Unidad,
           @CantidadConvertida, @MedidaBasica, @MedidaOpuesta, @PrecioMinimo, @PrecioMaximo, @Precio, @Total,
           @CalculoId, @UserCreate, GETDATE(), @SearchText
       );

SELECT SCOPE_IDENTITY() AS Id; -- Retorna el ID del registro insertado
END;