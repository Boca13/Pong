Module Puntuaciones
    'Puntuaciones
    Public Structure puntuacion
        Dim nombre As String
        Dim tiempo As UInt16
        Dim puntuacion As UInt32
    End Structure

    Public puntuaciones As List(Of puntuacion) = New List(Of puntuacion) 'Lista enlazada

    Sub guardarPuntuaciones()
        Dim c As Integer
        Dim s As String = ""
        If (Puntuaciones.Count > 0) Then
            While (c < Puntuaciones.Count - 1)
                s += Puntuaciones(c).nombre + ";" + Puntuaciones(c).tiempo.ToString + ";" + Puntuaciones(c).puntuacion.ToString + Chr(13) + Chr(10)
                c += 1
            End While
            s += Puntuaciones(c).nombre + ";" + Puntuaciones(c).tiempo.ToString + ";" + Puntuaciones(c).puntuacion.ToString
        End If
        My.Computer.FileSystem.WriteAllText(My.Application.Info.DirectoryPath + "/puntuaciones.csv", s, False)

    End Sub

    Sub cargarPuntuaciones()
        Try
            If (My.Computer.FileSystem.FileExists(My.Application.Info.DirectoryPath + "\puntuaciones.csv")) Then
                Dim reader As FileIO.TextFieldParser = New FileIO.TextFieldParser(My.Application.Info.DirectoryPath + "\puntuaciones.csv")
                Dim buffer(2) As String
                reader.TextFieldType = FileIO.FieldType.Delimited
                reader.SetDelimiters(";")
                Dim buffP As puntuacion

                While (Not reader.EndOfData)
                    buffer = reader.ReadFields()
                    buffP = New puntuacion()
                    buffP.nombre = buffer(0)
                    buffP.tiempo = buffer(1)
                    buffP.puntuacion = buffer(2)
                    puntuaciones.Add(buffP)
                End While
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

    End Sub

End Module
