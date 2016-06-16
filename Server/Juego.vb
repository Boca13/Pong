Imports System.Net
Public Class Juego
    Dim cliente As Sockets.Socket
    Dim flujo As Sockets.NetworkStream
    Public id As Integer

    Dim WithEvents tout As Timers.Timer = New Timers.Timer(timeout)

    Const key As String = "PONG"
    Const timeout As Integer = 300
    Const intervalo As Integer = 5000
    Const vDificultad As Integer = -5

    Structure Paquete
        Dim tipo As Char
        Dim datos As String
    End Structure

    Sub New(ByRef c As Sockets.Socket, Optional _id As Integer = 0)
        cliente = c
        cliente.Blocking = True
        id = _id
    End Sub


    Sub jugar()
        Try

        
        Dim bienvenida As String = key + "b" + My.Resources.MBienvenida1 + My.Computer.Clock.LocalTime.ToLongDateString + Chr(13) + Chr(10) + My.Computer.Clock.LocalTime.ToLocalTime + Chr(13) + Chr(10) + My.Resources.MBienvenida2 + Chr(0)

        Dim buffer() As Byte = System.Text.Encoding.UTF8.GetBytes(bienvenida)
        Dim bufferS As String = ""

        cliente.Send(buffer)

        'Esperar START
        ReDim buffer(500)
        While (Not bufferS.Contains(key + "s")) 'Mientras no llegue el START
            cliente.Receive(buffer)
            bufferS += System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length)
            Threading.Thread.Sleep(1000)
        End While
        ReDim buffer(600)

        Dim leido1 As Integer = 0
        bufferS = ""
        leido1 += cliente.Receive(buffer)
        Dim clave() As Byte = System.Text.Encoding.UTF8.GetBytes("PONGp")
        Dim c(5) As Integer
        Dim leidoantes As Integer = leido1
            While (1)
                'Encontrar cabecera
                c(0) = System.Array.IndexOf(Of Byte)(buffer, clave(0))
                c(1) = System.Array.IndexOf(Of Byte)(buffer, clave(1))
                c(2) = System.Array.IndexOf(Of Byte)(buffer, clave(2))
                c(3) = System.Array.IndexOf(Of Byte)(buffer, clave(3))
                c(4) = System.Array.IndexOf(Of Byte)(buffer, clave(4))
                bufferS = System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length)
                If c(0) >= 0 And c(1) = (c(0) + 1) And c(2) = (c(1) + 1) And c(3) = (c(2) + 1) And c(4) = (c(3) + 1) Then 'Recibe puntuación
                    While (cliente.Available)
                        leido1 += cliente.Receive(buffer, leido1, buffer.Length - leido1, Sockets.SocketFlags.None)
                    End While
                    bufferS = System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length)
                    c(5) = c(4) + 1
                    Dim p As Puntuaciones.puntuacion
                    p.nombre = System.Text.Encoding.UTF8.GetString(buffer, c(5), 16)
                    p.tiempo = BitConverter.ToUInt16(buffer, c(5) + 16)
                    p.puntuacion = BitConverter.ToUInt32(buffer, c(5) + 18)
                    Puntuaciones.puntuaciones.Add(p)
                    Exit While

                End If


                leido1 += cliente.Receive(buffer, leido1, buffer.Length - leido1, Sockets.SocketFlags.None)
            End While
            Threading.Thread.Sleep(5000)
        Catch ex As Exception
            cliente.Disconnect(False)
        End Try

        dispose()

    End Sub


    Sub dispose() Handles tout.Elapsed
        tout.Stop()
        cliente.Dispose()
    End Sub
End Class
