using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFCore_Semana6.Migrations
{
    /// <inheritdoc />
    public partial class AddStoreProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                        migrationBuilder.Sql("""
                CREATE PROCEDURE reasignar_tareas(
                    p_usuario_origen_id integer,
                    p_usuario_destino_id integer
                )
                LANGUAGE plpgsql
                AS $procedure$
                BEGIN
                    IF p_usuario_origen_id = p_usuario_destino_id THEN
                        RAISE EXCEPTION 'Los usuarios de origen y destino deben ser diferentes.'
                            USING ERRCODE = '22023';
                    END IF;

                    IF NOT EXISTS (
                        SELECT 1 FROM "Usuarios" WHERE "Id" = p_usuario_origen_id
                    ) OR NOT EXISTS (
                        SELECT 1 FROM "Usuarios" WHERE "Id" = p_usuario_destino_id
                    ) THEN
                        RAISE EXCEPTION 'El usuario de origen o de destino no existe.'
                            USING ERRCODE = '23503';
                    END IF;

                    UPDATE "Tareas"
                    SET "ResponsableId" = p_usuario_destino_id
                    WHERE "ResponsableId" = p_usuario_origen_id;
                END;
                $procedure$;
                """);


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DROP PROCEDURE IF EXISTS reasignar_tareas(integer, integer);");


        }
    }
}
