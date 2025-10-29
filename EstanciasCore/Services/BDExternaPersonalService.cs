using Castle.Core;
using DAL.Data;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using OfficeOpenXml.Style;
using Org.BouncyCastle.Asn1.X509;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Policy;
using DAL.DTOs.Servicios;
using SixLabors.ImageSharp;
using DAL.Models;
//using static TheArtOfDev.HtmlRenderer.Adapters.RGraphicsPath;

namespace EstanciasCore.Services
{
    public class BDExternaPersonalService
    {
        private string connectionStringMySqlProduccionloan = @"Server=52.175.204.76; Port=3316; Database=loan; user=cpeexterno_lectura; password=3CXZ1LmJZ1;charset=utf8";


        private EstanciasContext _context;
        public BDExternaPersonalService(EstanciasContext context)
        {
            _context = context;
        }

        
        public List<PersonaLoan> getPersonaloanByEmail(string email)
        {
            try
            {
                List<PersonaLoan> persona = new List<PersonaLoan>();
                using (MySqlConnection conexion = new MySqlConnection(connectionStringMySqlProduccionloan))
                {
                    conexion.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conexion;
                    //cmd.CommandText = "SELECT Documento,nombre, TRIM(u.cod_unisup) AS cod_unisup,\r\nTRIM(u.mne_unidad) AS abreviatura, TRIM(u.desc_larga)
                    //AS descripcion,TRIM(u.codigo_op) AS cod_unidad_op\r\n,(SELECT COUNT(p.dni) FROM dbo.vdatos_head_dircom AS p WHERE p.pres_grado<'00600'
                    //AND p.destino=u.cod_unidad) AS of_sup\r\n,(SELECT COUNT(p.dni) FROM dbo.vdatos_head_dircom AS p WHERE p.pres_grado>='00600'
                    //AND p.pres_grado<'00800' AND p.destino=u.cod_unidad) AS of_jefe\r\n,(SELECT COUNT(p.dni) FROM dbo.vdatos_head_dircom AS p
                    //WHERE p.pres_grado>='00800' AND p.pres_grado<'01700' AND p.destino=u.cod_unidad) AS of_subal\r\n,(SELECT COUNT(p.dni)
                    //FROM dbo.vdatos_head_dircom AS p WHERE p.pres_grado>='01700' AND p.pres_grado<'02100' AND p.destino=u.cod_unidad)
                    //AS subof_sup\r\n,(SELECT COUNT(p.dni) FROM dbo.vdatos_head_dircom AS p WHERE p.pres_grado>='02100' AND p.pres_grado<'02700'
                    //AND p.destino=u.cod_unidad) AS subof_subal\r\nFROM dbo.velementos_dircom AS u WHERE u.cod_unidad NOT LIKE 'H%';";
                    //cmd.CommandText = "SELECT SUBSTRING(nombre, LOCATE(',', nombre)+1,LENGTH(nombre)) as nombres,SUBSTRING(nombre, 1, LOCATE(',', nombre) - 1) as apellido, Documento, email, numeroTarjetaCompleto   FROM persona where email = '"+email+"';";
                    //cmd.CommandText = "SELECT * FROM persona;";
                    cmd.CommandText = "SELECT p.Id AS PersonaId, t.Id AS IdTarjeta, SUBSTRING_INDEX(p.nombre, ',', 1) AS Apellido, TRIM(SUBSTRING_INDEX(p.nombre, ',', -1)) AS Nombres, p.Documento, p.email, t.numeroTarjetaCompleto FROM persona AS p INNER JOIN tarjeta AS t ON t.idPersona = p.id WHERE email = '"+email+"' AND t.idEstadoTarjeta=1;";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            persona.Add(
                            new PersonaLoan()
                            {
                                NroDocumento = reader["Documento"].ToString(),
                                Apellido = reader["apellido"].ToString(),
                                Nombres = reader["nombres"].ToString(),
                                Email = reader["email"].ToString(),
                                NroTarjeta = reader["numeroTarjetaCompleto"].ToString(),
                            });
                        }
                    }
                    conexion.Close();
                }
                if (persona.Count > 0)
                {
                    return persona;
                }
                else
                {
                    return null;
                }


            }
            catch (Exception ex)
            {
                return null;
            }


        }

        public List<PersonaLoan> getPersonaloan(string NroDocumento)
        {
            try
            {
                List<PersonaLoan> persona = new List<PersonaLoan>();
                using (MySqlConnection conexion = new MySqlConnection(connectionStringMySqlProduccionloan))
                {
                    conexion.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conexion;
                    //cmd.CommandText = "SELECT Documento,nombre, TRIM(u.cod_unisup) AS cod_unisup,\r\nTRIM(u.mne_unidad) AS abreviatura, TRIM(u.desc_larga)
                    //AS descripcion,TRIM(u.codigo_op) AS cod_unidad_op\r\n,(SELECT COUNT(p.dni) FROM dbo.vdatos_head_dircom AS p WHERE p.pres_grado<'00600'
                    //AND p.destino=u.cod_unidad) AS of_sup\r\n,(SELECT COUNT(p.dni) FROM dbo.vdatos_head_dircom AS p WHERE p.pres_grado>='00600'
                    //AND p.pres_grado<'00800' AND p.destino=u.cod_unidad) AS of_jefe\r\n,(SELECT COUNT(p.dni) FROM dbo.vdatos_head_dircom AS p
                    //WHERE p.pres_grado>='00800' AND p.pres_grado<'01700' AND p.destino=u.cod_unidad) AS of_subal\r\n,(SELECT COUNT(p.dni)
                    //FROM dbo.vdatos_head_dircom AS p WHERE p.pres_grado>='01700' AND p.pres_grado<'02100' AND p.destino=u.cod_unidad)
                    //AS subof_sup\r\n,(SELECT COUNT(p.dni) FROM dbo.vdatos_head_dircom AS p WHERE p.pres_grado>='02100' AND p.pres_grado<'02700'
                    //AND p.destino=u.cod_unidad) AS subof_subal\r\nFROM dbo.velementos_dircom AS u WHERE u.cod_unidad NOT LIKE 'H%';";
                    
                    //cmd.CommandText = "SELECT SUBSTRING(p.nombre, LOCATE(',', p.nombre)+1,LENGTH(p.nombre)) as p.nombres,SUBSTRING(p.nombre, 1, LOCATE(',', p.nombre) - 1) as p.apellido, p.Documento, p.email, t.numeroTarjetaCompleto  FROM persona as p" +
                    //    "inner join tarjeta as t on t.idPersona=p.id" +
                    //    "where Documento = '"+NroDocumento+"';";

                    cmd.CommandText = "SELECT p.Id AS PersonaId, t.Id AS IdTarjeta, SUBSTRING_INDEX(p.nombre, ',', 1) AS Apellido, TRIM(SUBSTRING_INDEX(p.nombre, ',', -1)) AS Nombres, p.Documento, p.email, t.numeroTarjetaCompleto FROM persona AS p INNER JOIN tarjeta AS t ON t.idPersona = p.id WHERE Documento = '"+NroDocumento+"' AND (t.idEstadoTarjeta=1 OR t.idEstadoTarjeta=8);";


                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            persona.Add(
                            new PersonaLoan()
                            {
                                NroDocumento = reader["Documento"].ToString(),
                                Apellido = reader["apellido"].ToString(),
                                Nombres = reader["nombres"].ToString(),
                                Email = reader["email"].ToString(),                                
                                NroTarjeta = reader["numeroTarjetaCompleto"].ToString(),                                
                            });
                        }
                    }
                    conexion.Close();
                }
                if (persona.Count > 0)
                {
                    return persona;
                }
                else 
                {
                    return null;
                }
                   
                
            }
            catch (Exception ex)
            {
                return null;
            }
            

        }
        public List<PersonaLoan> getNroTarjetaloan(string nroDocumento)
        {
            try
            {
                List<PersonaLoan> persona = new List<PersonaLoan>();
                using (MySqlConnection conexion = new MySqlConnection(connectionStringMySqlProduccionloan))
                {
                    conexion.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conexion;
                    //cmd.CommandText = "SELECT Documento,nombre, TRIM(u.cod_unisup) AS cod_unisup,\r\nTRIM(u.mne_unidad) AS abreviatura, TRIM(u.desc_larga)
                    //AS descripcion,TRIM(u.codigo_op) AS cod_unidad_op\r\n,(SELECT COUNT(p.dni) FROM dbo.vdatos_head_dircom AS p WHERE p.pres_grado<'00600'
                    //AND p.destino=u.cod_unidad) AS of_sup\r\n,(SELECT COUNT(p.dni) FROM dbo.vdatos_head_dircom AS p WHERE p.pres_grado>='00600'
                    //AND p.pres_grado<'00800' AND p.destino=u.cod_unidad) AS of_jefe\r\n,(SELECT COUNT(p.dni) FROM dbo.vdatos_head_dircom AS p
                    //WHERE p.pres_grado>='00800' AND p.pres_grado<'01700' AND p.destino=u.cod_unidad) AS of_subal\r\n,(SELECT COUNT(p.dni)
                    //FROM dbo.vdatos_head_dircom AS p WHERE p.pres_grado>='01700' AND p.pres_grado<'02100' AND p.destino=u.cod_unidad)
                    //AS subof_sup\r\n,(SELECT COUNT(p.dni) FROM dbo.vdatos_head_dircom AS p WHERE p.pres_grado>='02100' AND p.pres_grado<'02700'
                    //AND p.destino=u.cod_unidad) AS subof_subal\r\nFROM dbo.velementos_dircom AS u WHERE u.cod_unidad NOT LIKE 'H%';";
                    //cmd.CommandText = "SELECT p.Documento,t.numeroTarjetaCompleto,Date(Now()) as fecha,SUBSTRING(p.nombre, LOCATE('','', p.nombre)+1,LENGTH(p.nombre)) as nom,SUBSTRING(p.nombre, 1, LOCATE('','', p.nombre) - 1) as ape, FechaNacimiento,email FROM TARJETA t INNER JOIN PERSONA p on t.idPersona=p.id where fechaVencimiento >= Date(Now()) and t.idEstadoTarjeta=1 and p.Documento = " + nroDocumento + ";";
                    
                    cmd.CommandText = "SELECT p.Id AS PersonaId, t.Id AS IdTarjeta, SUBSTRING_INDEX(p.nombre, ',', 1) AS Apellido, TRIM(SUBSTRING_INDEX(p.nombre, ',', -1)) AS Nombres, p.Documento, p.email, p.FechaNacimiento,t.numeroTarjetaCompleto FROM persona AS p INNER JOIN tarjeta AS t ON t.idPersona = p.id WHERE Documento = '"+nroDocumento+"' AND (t.idEstadoTarjeta=1 OR t.idEstadoTarjeta=8);";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            persona.Add(
                            new PersonaLoan()
                            {
                                NroDocumento = reader["Documento"].ToString(),
                                NroTarjeta= reader["numeroTarjetaCompleto"].ToString(),
                                Apellido = reader["apellido"].ToString(),
                                Nombres = reader["nombres"].ToString(),
                                Email = reader["email"].ToString(),
                                FechaNacimiento = reader["FechaNacimiento"].ToString()

                            });
                        }
                    }
                    conexion.Close();
                }
                if (persona.Count > 0)
                {
                    return persona;
                }
                else
                {
                    return null;
                }


            }
            catch (Exception ex)
            {
                return null;
            }


        }



        public List<PersonaLoan> getPersonaloanByNroTarjeta(string nroTarjeta)
        {
            try
            {
                List<PersonaLoan> persona = new List<PersonaLoan>();
                using (MySqlConnection conexion = new MySqlConnection(connectionStringMySqlProduccionloan))
                {
                    conexion.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conexion;
					//cmd.CommandText = "SELECT Documento,nombre, TRIM(u.cod_unisup) AS cod_unisup,\r\nTRIM(u.mne_unidad) AS abreviatura, TRIM(u.desc_larga)
					//AS descripcion,TRIM(u.codigo_op) AS cod_unidad_op\r\n,(SELECT COUNT(p.dni) FROM dbo.vdatos_head_dircom AS p WHERE p.pres_grado<'00600'
					//AND p.destino=u.cod_unidad) AS of_sup\r\n,(SELECT COUNT(p.dni) FROM dbo.vdatos_head_dircom AS p WHERE p.pres_grado>='00600'
					//AND p.pres_grado<'00800' AND p.destino=u.cod_unidad) AS of_jefe\r\n,(SELECT COUNT(p.dni) FROM dbo.vdatos_head_dircom AS p
					//WHERE p.pres_grado>='00800' AND p.pres_grado<'01700' AND p.destino=u.cod_unidad) AS of_subal\r\n,(SELECT COUNT(p.dni)
					//FROM dbo.vdatos_head_dircom AS p WHERE p.pres_grado>='01700' AND p.pres_grado<'02100' AND p.destino=u.cod_unidad)
					//AS subof_sup\r\n,(SELECT COUNT(p.dni) FROM dbo.vdatos_head_dircom AS p WHERE p.pres_grado>='02100' AND p.pres_grado<'02700'
					//AND p.destino=u.cod_unidad) AS subof_subal\r\nFROM dbo.velementos_dircom AS u WHERE u.cod_unidad NOT LIKE 'H%';";
					//cmd.CommandText = "SELECT p.Documento,t.numeroTarjetaCompleto,Date(Now()) as fecha,SUBSTRING(p.nombre, LOCATE('','', p.nombre)+1,LENGTH(p.nombre)) as nom,SUBSTRING(p.nombre, 1, LOCATE('','', p.nombre) - 1) as ape, FechaNacimiento,email FROM TARJETA t INNER JOIN PERSONA p on t.idPersona=p.id where fechaVencimiento >= Date(Now()) and t.idEstadoTarjeta=1 and t.numeroTarjetaCompleto = " + nroTarjeta + ";";
					
					long numeroSinCeros = long.Parse(nroTarjeta.TrimStart('0'));
					cmd.CommandText = "SELECT p.Documento,t.numeroTarjetaCompleto,Date(Now()) as fecha,SUBSTRING(p.nombre, LOCATE('','', p.nombre)+1,LENGTH(p.nombre)) as nom,SUBSTRING(p.nombre, 1, LOCATE('','', p.nombre) - 1) as ape, FechaNacimiento,email FROM TARJETA t INNER JOIN PERSONA p on t.idPersona=p.id where (t.idEstadoTarjeta=1 OR t.idEstadoTarjeta=8) and t.numeroTarjeta = " + numeroSinCeros + ";";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            persona.Add(
                            new PersonaLoan()
                            {
                                NroDocumento = reader["Documento"].ToString(),
                                NroTarjeta= reader["numeroTarjetaCompleto"].ToString(),
                                Apellido = reader["ape"].ToString(),
                                Nombres = reader["nom"].ToString(),
                                Email = reader["email"].ToString(),
                                FechaNacimiento = reader["FechaNacimiento"].ToString()

                            });
                        }
                    }
                    conexion.Close();
                }
                if (persona.Count > 0)
                {
                    return persona;
                }
                else
                {
                    return null;
                }


            }
            catch (Exception ex)
            {
                return null;
            }


        }
    }
}
