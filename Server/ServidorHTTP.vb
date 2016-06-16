Imports System.Net.Sockets
Imports System.Threading

Class ServidorHTTP
    Dim listener As TcpListener
    Dim hilo As Thread

    Sub New(l As TcpListener)
        listener = l
        hilo = New Thread(AddressOf servir)
        hilo.Start()
    End Sub

    Public Sub servir()
        Dim cliente As Socket
        listener.Start()
        While (1)
            cliente = listener.AcceptSocket()
            cliente.Blocking = True
            Dim html As String = My.Resources.HTML1
            Dim http As String = My.Resources.CabeceraHTTP
            Dim c As Integer = 0
            While (c < Puntuaciones.puntuaciones.Count)
                html += "<tr><td>" + Puntuaciones.puntuaciones(c).nombre + "</td><td>" + Puntuaciones.puntuaciones(c).tiempo.ToString + "</td><td>" + Puntuaciones.puntuaciones(c).puntuacion.ToString + "</td></tr>"
                c += 1
            End While

            html += My.Resources.HTML2
            Dim length As Integer = html.Length
            http += length.ToString + Chr(13) + Chr(10) + Chr(13) + Chr(10)
            http += html
            Dim buffer() As Byte = System.Text.Encoding.UTF8.GetBytes(http)
            cliente.Send(buffer)

            cliente.Disconnect(False)
            cliente.Close()
        End While
    End Sub

    Sub Abort()
        listener.Stop()
        hilo.Abort()
    End Sub
End Class