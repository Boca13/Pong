Imports System.Net
Imports System.Threading

Public Class Servidor
    Dim listener As Sockets.TcpListener
    Dim HTTPlistener As Sockets.TcpListener
    Dim encendido As Boolean = False
    Dim juegos As List(Of Juego) = New List(Of Juego)
    Dim hilos As List(Of Thread) = New List(Of Thread)
    Dim HTTPserver As ServidorHTTP


    Private Sub Servidor_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim host As IPHostEntry = Dns.GetHostEntry(Dns.GetHostName())
        ComboBox1.Items.AddRange(host.AddressList)
        ComboBox1.SelectedIndex = 2
        cargarPuntuaciones()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            If encendido = False Then
                listener = New Sockets.TcpListener(IPAddress.Parse(ComboBox1.Text), TextBox2.Text)
                HTTPlistener = New Sockets.TcpListener(IPAddress.Parse(ComboBox1.Text), 80)
                If (listener IsNot Nothing) And (HTTPlistener IsNot Nothing) Then
                    listener.Start()

                    'Arranca un hilo para el servidor HTTP
                    HTTPserver = New ServidorHTTP(HTTPlistener)

                    Label5.ForeColor = Color.Green
                    Label5.Text = "Encendido"
                    Button1.Text = "Desconectar"
                    encendido = True
                    TextBox2.Enabled = False
                    ComboBox1.Enabled = False
                    Timer1.Interval = 100
                    NumericUpDown1.Enabled = False
                    Timer1.Start()
                End If
                encendido = True
            Else
                Timer1.Stop()
                listener.Stop()
                For Each hilo As Thread In hilos
                    hilo.Abort()
                Next
                For Each j As Juego In juegos
                    j.dispose()
                Next
                hilos.Clear()
                juegos.Clear()
                HTTPserver.Abort()
                Label5.ForeColor = Color.Red
                Button1.Text = "Conectar"
                Label5.Text = "Apagado"
                ListView1.Items.Clear()
                encendido = False
                NumericUpDown1.Enabled = True
                TextBox2.Enabled = True
                ComboBox1.Enabled = True
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If listener.Pending() Then
            If juegos.Count < NumericUpDown1.Value Then 'Si no se ha llegado al máximo
                Dim cliente As Sockets.Socket = listener.AcceptSocket() 'Aceptar conexión
                Dim endpoint As IPEndPoint = cliente.RemoteEndPoint 'Client.RemoteEndPoint
                Dim elemento As ListViewItem = New ListViewItem(endpoint.Address.ToString, 0) 'Crear elemento para la lista: IP
                Dim j As Juego = New Juego(cliente, ListView1.Items.Count) 'Crear una instancia de Juego

                'Para añadir a la lista:
                elemento.SubItems.Add(endpoint.Port) 'Puerto
                elemento.SubItems.Add(My.Computer.Clock.LocalTime.TimeOfDay.ToString) 'Hora de conexión
                ListView1.Items.Add(elemento) 'Añadir cliente a la lista

                'Crear hilo para este cliente
                Dim hilo As Thread = New Thread(AddressOf j.jugar)
                juegos.Add(j)
                hilos.Add(hilo) 'Crear un hilo para ese juego
                hilo.Start()
                Label2.Text = juegos.Count
            ElseIf juegos.Count = NumericUpDown1.Value - 3 And Timer1.Interval = 10000 Then 'Casi al límite porque ha bajado del máximo
                Timer1.Interval = 100 'Se devuelve el temporizador al intervalo original
            Else 'Se ha llegado al máximo de usuarios
                Timer1.Interval = 10000 'Se reduce la frecuencia de comprobación de peticiones para mejorar el rendimiento
            End If
        End If

        Try
            'Eliminar conexiones terminadas
            For c As Integer = 0 To hilos.Count - 1
                If (hilos(c).ThreadState = ThreadState.Stopped) Then
                    ListView1.Items(juegos(c).id).Remove()
                    juegos.Remove(juegos(c))
                    hilos.Remove(hilos(c))
                    Label2.Text = ListView1.Items.Count.ToString()
                End If
            Next

        Catch ex As Exception
        End Try
    End Sub

    Private Sub ListView1_ControlAdded(sender As Object, e As ControlEventArgs) Handles ListView1.ControlAdded, ListView1.ControlRemoved
        Label2.Text = ListView1.Items.Count
    End Sub

    Private Sub Servidor_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        For Each hilo As Thread In hilos
            hilo.Abort()
        Next
        For Each j As Juego In juegos
            j.dispose()
        Next
        If Not (HTTPserver Is Nothing) Then HTTPserver.Abort()
        If Not (listener Is Nothing) Then listener.Stop()

        Puntuaciones.guardarPuntuaciones()
    End Sub

    Delegate Sub eliminarClienteInvoker(i As Integer)

    Sub eliminarCliente(i As Integer)
        If ListView1.InvokeRequired Then
            Me.ListView1.Invoke(New eliminarClienteInvoker(AddressOf eliminarCliente), i)
        Else
            ListView1.Items(i).Remove()
            Label2.Text = ListView1.Items.Count.ToString()
        End If

    End Sub

End Class
