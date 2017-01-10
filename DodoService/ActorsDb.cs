using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Sql;
using System.Collections.Specialized;

namespace DodoService
{
    public class ActorsDb
    {
        public void OpenDatabase(string connectionString)
        {
            // connectionString = @"Data Source=.\SQLEXPRESS;AttachDbFilename=C:\datos\develop\DodoServer\DodoService\App_Data\Database.mdf;Integrated Security=True;User Instance=True";
            conn = new SqlConnection(connectionString);
            conn.Open();
        }

        public int AddActor(NameValueCollection coll)
        {
            int ret = 0;
            object tmp = null;
            SqlCommand cmd = conn.CreateCommand();

            string sql = "insert into actors (";
            sql += "nombre, apellidos, registro) values (";
            sql += "@nombre, @apellidos, @registro); ";
            sql += "SELECT SCOPE_IDENTITY() AS id";

            AddParameter(cmd, "nombre", "nuevo");
            AddParameter(cmd, "apellidos", "nuevo");
            AddParameter(cmd, "registro", DateTime.Now);

            cmd.CommandText = sql;
            tmp = cmd.ExecuteScalar();
            ret = Convert.ToInt32(tmp);
            return ret;
        }

        public void RemoveActor(string id)
        {
            try
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    string sql = "delete from actors where id = " + id;
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public int SetActor(NameValueCollection coll)
        {
            try
            {
                int ret = 0;
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    string sql = "update actors set ";
                    sql += "nombre=@nombre, apellidos=@apellidos, sexo=@sexo, edad=@edad, dni=@dni, ";
                    sql += "numss=@numss, direccion=@direccion, cp=@cp, poblacion=@poblacion, provincia=@provincia, nacionalidad=@nacionalidad, ";
                    sql += "altura=@altura, peso=@peso, ";
                    sql += "talla_camisa=@talla_camisa, talla_chaqueta=@talla_chaqueta, talla_pantalon=@talla_pantalon, talla_zapato=@talla_zapato, ";
                    sql += "talla_pecho=@talla_pecho, talla_cintura=@talla_cintura, talla_cadera=@talla_cadera, ";
                    sql += "color_ojos=@color_ojos, color_cabello=@color_cabello, tipo_cabello=@tipo_cabello, ";
                    sql += "manos=@manos, pies=@pies, actor_cine=@actor_cine, actor_publicidad=@actor_publicidad, modelo=@modelo, ";
                    sql += "habilidades=@habilidades, observaciones=@observaciones, ";
                    sql += "modificacion=@modificacion ";
                    sql += "where id=" + coll["id"];


                    AddParameter(cmd, "nombre", coll["nombre"]);
                    AddParameter(cmd, "apellidos", coll["apellidos"]);
                    AddParameter(cmd, "sexo", coll["sexo"]);
                    AddParameter(cmd, "edad", 10);
                    AddParameterInt(cmd, "dni", coll["dni"]);
                    AddParameter(cmd, "numss", coll["numss"]);
                    AddParameter(cmd, "direccion", coll["direccion"]);
                    AddParameter(cmd, "cp", coll["cp"]);
                    AddParameter(cmd, "poblacion", coll["poblacion"]);
                    AddParameter(cmd, "provincia", coll["provincia"]);
                    AddParameter(cmd, "nacionalidad", coll["nacionalidad"]);
                    AddParameterInt(cmd, "altura", coll["altura"]);
                    AddParameterInt(cmd, "peso", coll["peso"]);
                    AddParameterInt(cmd, "talla_pecho", coll["talla_pecho"]);
                    AddParameterInt(cmd, "talla_cintura", coll["talla_cintura"]);
                    AddParameterInt(cmd, "talla_cadera", coll["talla_cadera"]);
                    AddParameterInt(cmd, "talla_camisa", coll["talla_camisa"]);
                    AddParameterInt(cmd, "talla_chaqueta", coll["talla_chaqueta"]);
                    AddParameterInt(cmd, "talla_pantalon", coll["talla_pantalon"]);
                    AddParameterInt(cmd, "talla_zapato", coll["talla_zapato"]);
                    AddParameter(cmd, "color_ojos", coll["color_ojos"]);
                    AddParameter(cmd, "color_cabello", coll["color_cabello"]);
                    AddParameter(cmd, "tipo_cabello", coll["tipo_cabello"]);
                    AddParameterBool(cmd, "manos", coll["manos"]);
                    AddParameterBool(cmd, "pies", coll["pies"]);
                    AddParameterBool(cmd, "actor_cine", coll["actor_cine"]);
                    AddParameterBool(cmd, "actor_publicidad", coll["actor_publicidad"]);
                    AddParameterBool(cmd, "modelo", coll["modelo"]);
                    AddParameter(cmd, "habilidades", coll["habilidades"]);
                    AddParameter(cmd, "observaciones", coll["observaciones"]);
                    AddParameter(cmd, "modificacion", DateTime.Now);

                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();

                    ret = int.Parse(coll["id"]);
                }
                return ret;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public DataTable GetActors()
        {
            DataTable dt = null;
            using (SqlDataAdapter da = new SqlDataAdapter("select * from actors", conn))
            {
                dt = new DataTable();
                da.Fill(dt);
            }
            return dt;
        }

        public DataTable GetActor(string id)
        {
            DataTable dt = null;
            using (SqlDataAdapter da = new SqlDataAdapter("select * from actors where id = '" + id + "'", conn))
            {
                dt = new DataTable();
                da.Fill(dt);
            }
            return dt;
        }
    }
}
