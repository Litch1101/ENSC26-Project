Imports System.Numerics
Imports System.Reflection.Emit

Public Class Form3

    Private Sub btnConvert_Click(sender As Object, e As EventArgs) Handles btnConvert.Click
        Try
            Dim input1, input2 As Double
            input1 = txtin1.Text
            input2 = txtin2.Text
            Dim complexnum As Complex

            If rdbPolar.Checked Then
                complexnum = New Complex(input1, input2)
                txtout1.Text = Math.Round(complexnum.Magnitude, 4)
                txtout2.Text = Math.Round(complexnum.Phase, 4)

            ElseIf rdbRect.Checked Then
                complexnum = Complex.FromPolarCoordinates(input1, Math.PI * input2 / 180)
                txtout1.Text = Math.Round(complexnum.Real, 4)
                txtout2.Text = Math.Round(complexnum.Imaginary, 4)
            Else
                MessageBox.Show("Please choose a form to convert to.", "Error")
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub btnReset_Click() Handles btnReset.Click
        txtin2.Clear()
        txtout2.Clear()
        txtin1.Clear()
        txtout1.Clear()
        txtin1.Focus()
    End Sub

    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        Me.Close()
    End Sub

    Private Sub PolarChecked() Handles rdbPolar.CheckedChanged

        Label2.Text = "Rectangular Form:"
        Label3.Text = "Real:"
        Label4.Text = "Imaginary:"
        Label10.Text = "j"

        Label7.Text = "Polar Form:"
        Label6.Text = "Magnitude:"
        Label5.Text = "Angle:"
        Label8.Text = "°"

        Call btnReset_Click()
    End Sub

    Private Sub RectChecked() Handles rdbRect.CheckedChanged
        Label7.Text = "Rectangular Form:"
        Label6.Text = "Real:"
        Label5.Text = "Imaginary:"
        Label8.Text = "j"

        Label2.Text = "Polar Form:"
        Label3.Text = "Magnitude:"
        Label4.Text = "Angle:"
        Label10.Text = "°"

        Call btnReset_Click()
    End Sub
End Class