Imports MySql.Data.MySqlClient
Public Class Form1
    Dim bandera As Integer
    Private Sub Button1_Click(sender As Object, e As EventArgs)

    End Sub

    Sub cargardatagrid()
        Dim cString As String 'Creo la Cadena de Conexión
        cString = "server=localhost;user=root;
                   database=biblioteca;port=3306;
                   password=CVO2023;"
        'Crear Conexión
        Dim conn As New MySqlConnection(cString)

        Try
            'Abrimos la conexión
            conn.Open()
            'Crear cadena de Consulta
            Dim sQuery As String
            sQuery = "SELECT a.idautor, 
                    CONCAT(a.nombre, ' ', a.apellidos) 
                    as 'Nombre', a.fecha_nac, 
                    a.fecha_muerte, a.pais FROM autor a;" 'Consulta SQL
            'Se crea el Data Adapter
            Dim da As New MySqlDataAdapter(sQuery, conn)
            'creamos un DataTable
            Dim dt As New DataTable
            'Llenamos el DataTable con el Adaptador
            da.Fill(dt)
            'Llenamos el DataGrid con el DataTable
            DataGridView1.DataSource = dt
            DataGridView1.Refresh()
            conn.Close() 'Cerramos la conexión
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString())
        End Try
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        cargardatagrid()
        DateTimePicker2.Enabled = False
        bandera = -1
    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        Dim nombre, apellidos, pais As String
        Dim fecha_nac As Date
        Dim fecha_muerte As Nullable(Of Date)
        Dim sQuery As String
        nombre = TextBox1.Text
        apellidos = TextBox2.Text
        pais = TextBox3.Text
        fecha_nac = DateTimePicker1.Value.Date
        If CheckBox1.Checked Then
            fecha_muerte = DateTimePicker2.Value.Date
        Else
            fecha_muerte = Nothing
        End If


        Dim cString As String 'Creo la Cadena de Conexión
        cString = "server=localhost;user=root;
                   database=biblioteca;port=3306;
                   password=CVO2023;"
        'Crear Conexión
        Dim conn As New MySqlConnection(cString)
        Try
            conn.Open()
            Dim cm As New MySqlCommand
            If bandera = -1 Then
                sQuery = "INSERT INTO autor(nombre, apellidos,fecha_nac, 
fecha_muerte, pais) VALUES(@nombre, @apellidos, @fecha_nac, 
@fecha_muerte , @pais);"

            Else
                sQuery = "UPDATE autor SET nombre = @nombre, 
apellidos = @apellidos, fecha_nac = @fecha_nac, fecha_muerte = @fecha_muerte, 
pais = @pais WHERE idautor = " & bandera & ";"
                bandera = -1
                Button1.Text = "Agregar"
            End If
            cm.Connection() = conn
            cm.CommandText() = sQuery
            cm.Parameters.AddWithValue("@nombre", nombre)
            cm.Parameters.AddWithValue("@apellidos", apellidos)
            cm.Parameters.AddWithValue("@fecha_nac", fecha_nac)
            cm.Parameters.AddWithValue("@fecha_muerte", fecha_muerte)
            cm.Parameters.AddWithValue("@pais", pais)
            cm.ExecuteNonQuery()
            MessageBox.Show("Guardado con Éxito!")
            cargardatagrid()
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString())
        End Try
        limpiar()
        Button2.Enabled = False
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        Dim idautor As Integer
        idautor = DataGridView1.CurrentRow.Cells(0).Value
        Dim sQuery As String
        sQuery = "SELECT * FROM autor WHERE idautor=" & idautor & ";"
        Dim cString As String 'Creo la Cadena de Conexión
        cString = "server=localhost;user=root;
                   database=biblioteca;port=3306;
                   password=CVO2023;"
        'Crear Conexión
        bandera = idautor
        Try
            Dim conn As New MySqlConnection(cString)
            Dim da As New MySqlDataAdapter(sQuery, conn)
            Dim dt As New DataTable()
            da.Fill(dt)
            If dt.Rows.Count > 0 Then
                Dim fila As DataRow = dt.Rows(0)
                TextBox1.Text = fila("nombre").ToString()
                TextBox2.Text = fila("apellidos").ToString()
                TextBox3.Text = fila("pais").ToString()
                DateTimePicker1.Value = Convert.ToDateTime(fila("fecha_nac")).Date
                If fila("fecha_muerte") Is DBNull.Value Then
                    CheckBox1.Checked = False
                    DateTimePicker2.Value = Now
                Else
                    DateTimePicker2.Value = Convert.ToDateTime(fila("fecha_muerte")).Date
                    CheckBox1.Checked = True
                End If

            Else
                MessageBox.Show("No existe el autor")
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString())
        End Try
        Button1.Text = "Modificar"
        Button2.Enabled = True
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked Then
            DateTimePicker2.Enabled = True
        Else
            DateTimePicker2.Enabled = False
        End If
    End Sub

    Sub limpiar()
        TextBox1.Clear()
        TextBox2.Clear()
        TextBox3.Clear()
        DateTimePicker1.Value = Today
        DateTimePicker2.Value = Today
        CheckBox1.Checked = False
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            DialogResult = MessageBox.Show("Desea Eliminar el registro?", "Eliminar", MessageBoxButtons.YesNo)

            If DialogResult = vbYes Then
                Dim sQuery As String
                sQuery = "DELETE FROM autor WHERE idautor = " & bandera & ";"
                Dim cString As String 'Creo la Cadena de Conexión
                cString = "server=localhost;user=root;
                   database=biblioteca;port=3306;
                   password=CVO2023;"
                'Crear Conexión
                Dim conn As New MySqlConnection(cString)
                conn.Open()
                Dim cm As New MySqlCommand
                cm.Connection() = conn
                cm.CommandText() = sQuery
                cm.ExecuteNonQuery()
                MessageBox.Show("Eliminado con Éxito")
                cargardatagrid()
                bandera = -1
                Button2.Enabled = False
            Else
                bandera = -1
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString())
        End Try
        limpiar()
    End Sub
End Class
