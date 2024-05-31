using Common;
using DataAccess.DBServices.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DataAccess.DBServices
{
    public class UserDao : ConnectionToSql
    {
        /// <summary>
        /// Esta clase hereda de clase ConnectionToSql.
        /// Aquí se realiza las diferentes transacciones y consultas a la base de datos de la entidad
        /// usuario.
        /// </summary>
        /// 

        public User Login(string username, string password)
        {//Validar el usuario y contraseña del usuario para el inicio de sesion.

            using (var connection = GetConnection()) //Obtener la conexion.
            {
                connection.Open(); //Abrir la conexion.
                using (var command = new SqlCommand())//Crear objeto SqlCommand.
                {
                    command.Connection = connection;//Establecer la conexión.
                    //Establecer el comando de texto.
                    command.CommandText = "select *from Usuarios where (usuario=@username and contrasenia=@pass)";
                    //Establecer los parametros.
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@pass", password);
                    command.CommandType = CommandType.Text;//Establecer el tipo de comando.

                    SqlDataReader reader = command.ExecuteReader();//Ejecutar y establecer el lector.
                    if (reader.Read())//Si el lector tiene filas que leer.
                    {
                        var userObj = new User //Crear objeto y asignar los datos del resultado.
                        {
                            Id = (int)reader["id_usuario"],
                            Username = reader["usuario"].ToString(),
                        };
                        //Asignar los datos del usuario conectado actualmente en la aplicacion.
                        ActiveUser.Id = userObj.Id;                      
                        ActiveUser.Username = username;

                        return userObj; //Retornar el objeto usuario (Resultado)
                    }
                    else //Si la consulta no fue exitosa, retornar nulo.
                        return null;
                }
            }
        }
        public bool ValidateActiveUser()
        {//Seguridad de la aplicacion, utiliza este metodo para verificar que el usuario conectado sea valido.
            bool validUser = false;//Obtiene o establece si el usuario conectado es valido (Valor por defecto =falso).
            string activeUserPass = "";//Obtiene o establece la contraseña del usuario conectado.
            if (string.IsNullOrWhiteSpace(ActiveUser.Username) == false) //Ejecutar este fragmento de codigo siempre en cuando que el nombre usuario NO sea nulo o espacios en blanco.
            {
                using (var connection = GetConnection())//Obtener conexion
                {
                    connection.Open();
                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;
                        //Obtener la contraseña del usuario conectado.
                        command.CommandText = "select contrasenia from Usuarios where id_usuario =@id";//Establecer el comando de texto
                        command.Parameters.AddWithValue("@id", ActiveUser.Id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())//Si el lector tiene filas que leer, almacenar el resultado (Contraseña) en el campo activeUserPass.
                                activeUserPass = reader[0].ToString();
                            command.Parameters.Clear();//Limpiar los parametros para la siguiente consulta.
                        }
                        //Validar usuario conectado.
                        command.CommandText = "select *from Usuarios where usuario=@username and contrasenia=@pass and id_usuario=@id";
                        command.Parameters.AddWithValue("@username", ActiveUser.Username);
                        command.Parameters.AddWithValue("@pass", activeUserPass);
                        command.Parameters.AddWithValue("@id", ActiveUser.Id);

                        command.CommandType = CommandType.Text;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows) //Si el lector tiene filas, establecer validUser en verdadero.
                                validUser = true;
                            else //Caso contrario, establecer validUser en falso.
                                validUser = false;
                        }
                    }
                }
            }
            return validUser; //Retornar el resultado.
        }
        public int CreateUser(User user)
        {//Insertar nuevo usuario.
            int result = -1;

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    // Obtener el último id_usuario de la tabla Usuarios
                    command.CommandText = "SELECT MAX(id_usuario) FROM Usuarios";
                    int lastUserId = Convert.ToInt32(command.ExecuteScalar());

                    // Incrementar el último id_usuario para el nuevo usuario
                    int newUserId = lastUserId + 1;

                    // Insertar el nuevo usuario en la tabla Usuarios
                    command.CommandText = "INSERT INTO Usuarios (id_usuario, usuario, contrasenia) VALUES (@IdUsuario, @Usuario, @Contrasenia)";
                    command.Parameters.AddWithValue("@IdUsuario", newUserId);
                    command.Parameters.AddWithValue("@Usuario", user.Username);
                    command.Parameters.AddWithValue("@Contrasenia", user.Password);
                    result = command.ExecuteNonQuery();

                    // Si se insertó correctamente en Usuarios, insertar en Clientes
                    if (result > 0)
                    {
                        // Obtener el último id_usuario de la tabla Usuarios
                        command.CommandText = "SELECT MAX(id_usuario) FROM Usuarios";
                        int lastClientId = Convert.ToInt32(command.ExecuteScalar());

                        // Incrementar el último id_usuario para el nuevo usuario
                        int newClientId = lastClientId + 1;

                        // Insertar el nuevo cliente en la tabla Clientes
                        command.CommandText = "INSERT INTO Clientes (id_cliente, apellido_p, apellido_m, nombres, correo, telefono, cp, calle, num_calle, colonia, id_ciudad, id_usuario) " +
                                              "VALUES (@IdCliente, @ApellidoP, @ApellidoM, @Nombres, @Correo, @Telefono, @CP, @Calle, @NumCalle, @Colonia, @IdCiudad, @Id__Usuario)";
                        command.Parameters.AddWithValue("@IdCliente", newClientId); // Asignar el id_cliente proporcionado por el usuario
                        command.Parameters.AddWithValue("@ApellidoP", user.LastName);
                        command.Parameters.AddWithValue("@ApellidoM", user.MiddleName);
                        command.Parameters.AddWithValue("@Nombres", user.FirstName);
                        command.Parameters.AddWithValue("@Correo", user.Email);
                        command.Parameters.AddWithValue("@Telefono", user.Phone);
                        command.Parameters.AddWithValue("@CP", user.PostalCode);
                        command.Parameters.AddWithValue("@Calle", user.Street);
                        command.Parameters.AddWithValue("@NumCalle", user.StreetNumber);
                        command.Parameters.AddWithValue("@Colonia", user.Neighborhood);
                        command.Parameters.AddWithValue("@IdCiudad", user.CityId);
                        command.Parameters.AddWithValue("@Id__Usuario", newUserId); // Asignar el mismo id_usuario generado para el nuevo usuario en Usuarios
                        command.CommandType = CommandType.Text;
                        result = command.ExecuteNonQuery();
                    }

                    
                }
            }
            return result;//retornar el numero de filas afectadas de la transaccion. 
        }

        public int ModifyUser(User user)
        {//Actualizar usuario.
            int result = -1;

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE Usuarios
                                        SET usuario = @userName, contrasenia = @password
                                        WHERE id_usuario = @id";
                    command.Parameters.AddWithValue("@id", user.Id);
                    command.Parameters.AddWithValue("@userName", user.Username);
                    command.Parameters.AddWithValue("@password", user.Password);
                    result = command.ExecuteNonQuery();

                    command.CommandText = @"UPDATE Clientes
                                        SET apellido_p = @apellidoP, apellido_m = @apellidoM,
                                            nombres = @nombres, correo = @correo, telefono = @telefono,
                                            cp = @cp, calle = @calle, num_calle = @numCalle,
                                            colonia = @colonia, id_ciudad = @ciudadId
                                        WHERE id_usuario = @id";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@id", user.Id);
                    command.Parameters.AddWithValue("@apellidoP", user.LastName);
                    command.Parameters.AddWithValue("@apellidoM", user.MiddleName);
                    command.Parameters.AddWithValue("@nombres", user.FirstName);
                    command.Parameters.AddWithValue("@correo", user.Email);
                    command.Parameters.AddWithValue("@telefono", user.Phone);
                    command.Parameters.AddWithValue("@cp", user.PostalCode);
                    command.Parameters.AddWithValue("@calle", user.Street);
                    command.Parameters.AddWithValue("@numCalle", user.StreetNumber);
                    command.Parameters.AddWithValue("@colonia", user.Neighborhood);
                    command.Parameters.AddWithValue("@ciudadId", user.CityId);

                    command.CommandType = CommandType.Text;
                    result = command.ExecuteNonQuery();
                }
            }
            return result;
        }
        public int RemoveUser(int id)
        {//Eliminar usuario.
            int result = -1;

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT id_cliente FROM Clientes WHERE id_usuario = @id";
                    command.Parameters.AddWithValue("@id", id);
                    int clientId = (int)command.ExecuteScalar();
                    command.CommandText = "DELETE FROM Contratos WHERE id_cliente = @clientId";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@clientId", clientId);
                    result = command.ExecuteNonQuery();

                    command.CommandText = @"delete from Clientes where id_usuario=@id 
                    delete from Usuarios where id_usuario=@id ";
                    command.Parameters.AddWithValue("@id", id);
                
                    command.CommandType = CommandType.Text;
                    result = command.ExecuteNonQuery();
                }
            }
            return result;
        }
        public User GetUserById(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT u.id_usuario, u.usuario, u.contrasenia,
                                           c.apellido_p, c.apellido_m, c.nombres,
                                           c.correo, c.telefono, c.cp, c.calle,
                                           c.num_calle, c.colonia, c.id_ciudad
                                    FROM Usuarios u
                                    JOIN Clientes c ON u.id_usuario = c.id_usuario
                                    WHERE u.id_usuario = @id";
                    command.Parameters.AddWithValue("@id", id);
                    command.CommandType = CommandType.Text;

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        var userObj = new User
                        {
                            Id = (int)reader["id_usuario"],
                            Username = reader["usuario"].ToString(),
                            Password = reader["contrasenia"].ToString(),
                            FirstName = reader["nombres"].ToString(),
                            LastName = reader["apellido_p"].ToString(),
                            MiddleName = reader["apellido_m"].ToString(),
                            Email = reader["correo"].ToString(),
                            Phone = reader["telefono"].ToString(),
                            PostalCode = (int)reader["cp"],
                            Street = reader["calle"].ToString(),
                            StreetNumber = (int)reader["num_calle"],
                            Neighborhood = reader["colonia"].ToString(),
                            CityId = (int)reader["id_ciudad"]
                        };
                        return userObj; // Retornar resultado (objeto).
                    }
                    else
                    {
                        return null; // Retornar NULL si no hay resultado.
                    }
                }
            }
        }

        public User GetUserByUsername(string user)
        {//Obtener usuario por nombre de usuario o email.
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "select *from Usuarios where usuario=@user";
                    command.Parameters.AddWithValue("@user", user);
                    command.CommandType = CommandType.Text;

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        var userObj = new User
                        {
                            Id = (int)reader[0],
                            Username = reader[1].ToString(),
                            Password = reader[2].ToString(),
                        };
                        return userObj;
                    }
                    else
                        return null;
                }
            }
        }
        public IEnumerable<User> GetAllUsers()
        {//Listar usuarios.
            var userList = new List<User>();//Crear lista generica de usuarios.

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "select * from Usuarios ";
                    command.CommandType = CommandType.Text;

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())//Agregar los resultados en la lista mientras el lector siga leyendo las filas.
                        {
                            var userObj = new User
                            {
                                Id = (int)reader[0],
                                Username = reader[1].ToString(),
                                Password = reader[2].ToString(),
                            };
                            userList.Add(userObj);
                        }
                    }
                }
            }
            return userList; //Retornar lista.
        }
    }
}
