CREATE PROCEDURE DeleteAppCalculadora
    @Id INT
AS
BEGIN
DELETE FROM AppCalculadora
WHERE Id = @Id;
END;