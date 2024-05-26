Imports System.ComponentModel
Imports System.Numerics
Imports System.Windows.Forms.VisualStyles.VisualStyleElement

Public Class Form1
    Dim Resistance(2) As Double
    Dim Reactance(2) As Double
    Dim Vline, RealPower, ReactivePower, PowerFactor As Double
    Dim Zload As Complex() = New Complex(2) {}
    Dim Vphase As Complex() = New Complex(2) {}
    Dim VlineArray As Complex() = New Complex(2) {}
    Dim Iphase As Complex() = New Complex(2) {}
    Dim ApparentPower As New Complex
    Dim intXOrigin, intYOrigin As Integer
    Dim myfont As New Font("Arial", 8, FontStyle.Regular) 'Declaration of font
    Dim blnPlot As Boolean = False
    Dim zoomfac As Single = 1

    Private Sub btnCompute_Click(sender As Object, e As EventArgs) Handles btnCompute.Click
        Try
            If rdbBalanced.Checked = True Or rdbUnbalanced.Checked = True Then
                Vline = txtVline.Text
                txtVline.SelectAll()

                Try
                    Select Case True
                        Case rdbDelta.Checked And rdbBalanced.Checked 'yuri
                            txtR2.Text = txtR1.Text
                            txtR3.Text = txtR1.Text
                            txtX2.Text = txtX1.Text
                            txtX3.Text = txtX1.Text

                            Resistance(0) = txtR1.Text
                            Resistance(1) = txtR2.Text
                            Resistance(2) = txtR3.Text
                            Reactance(0) = txtX1.Text
                            Reactance(1) = txtX2.Text
                            Reactance(2) = txtX3.Text
                            For i As Integer = 0 To 2
                                Iphase(i) = DeltaBalanced(i)
                                Vphase(i) = Vphase(i)
                            Next
                            Dim magI As String = Iphase(0).Magnitude

                            RealPower = Math.Round((magI) ^ 2 * Resistance(0), 2)
                            ReactivePower = Math.Round((magI) ^ 2 * Reactance(0), 2)
                            ApparentPower = Math.Round(Math.Sqrt(RealPower ^ 2 + ReactivePower ^ 2), 2)
                            PowerFactor = Math.Round(RealPower / ApparentPower.Magnitude, 2)
                        Case rdbDelta.Checked And rdbUnbalanced.Checked 'rhenz
                            Resistance(0) = txtR1.Text
                            Resistance(1) = txtR2.Text
                            Resistance(2) = txtR3.Text
                            Reactance(0) = txtX1.Text
                            Reactance(1) = txtX2.Text
                            Reactance(2) = txtX3.Text
                            For i As Integer = 0 To 2
                                Zload(i) = New Complex(Resistance(i), Reactance(i))
                            Next
                            For i As Integer = 0 To 2
                                Vphase(i) = Complex.FromPolarCoordinates(Vline, (Math.PI * (i / 2) * 240) / 180)
                                Iphase(i) = Vphase(i) / Zload(i)
                                RealPower += Iphase(i).Magnitude ^ 2 * Resistance(i)
                                ReactivePower += Iphase(i).Magnitude ^ 2 * Reactance(i)
                                ApparentPower += Complex.Conjugate(Iphase(i)) * Vphase(i)
                            Next
                            PowerFactor = Math.Round(RealPower / ApparentPower.Magnitude, 2)
                            RealPower = Math.Round(RealPower, 2)
                            ReactivePower = Math.Round(ReactivePower, 2)
                        Case rdbWye.Checked And rdbBalanced.Checked 'zel
                            txtR2.Text = txtR1.Text
                            txtR3.Text = txtR1.Text
                            txtX2.Text = txtX1.Text
                            txtX3.Text = txtX1.Text
                            Resistance(0) = txtR1.Text
                            Resistance(1) = txtR2.Text
                            Resistance(2) = txtR3.Text
                            Reactance(0) = txtX1.Text
                            Reactance(1) = txtX2.Text
                            Reactance(2) = txtX3.Text
                            Call Wye_balanced()
                        Case rdbWye.Checked And rdbUnbalanced.Checked
                            Resistance(0) = txtR1.Text
                            Resistance(1) = txtR2.Text
                            Resistance(2) = txtR3.Text
                            Reactance(0) = txtX1.Text
                            Reactance(1) = txtX2.Text
                            Reactance(2) = txtX3.Text
                            Call Wye_Unbalanced()
                    End Select

                    Call output()

                    btnShow.Enabled = True
                    AcceptButton = btnShow
                    grpOutput.Enabled = True
                Catch ex As Exception
                    MsgBox("error")
                End Try
            Else
                MessageBox.Show("Please choose a load type.", "Input Error")
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub
    Function DeltaBalanced(ByVal x As Integer)
        Zload(x) = New Complex(Resistance(x), Reactance(x))
        If x = 0 Then
            Vphase(x) = Complex.FromPolarCoordinates(Vline, Math.PI * (0) / 180)
        Else
            Vphase(x) = Complex.FromPolarCoordinates(Vline, Math.PI * (240 / x) / 180)
        End If

        Dim ans = Vphase(x) / Zload(x)
        Return ans
    End Function

    Private Sub Wye_Unbalanced()
        'declaring variables lang to. bale diff sya sa balanced since dun ay equal lang sa r1 x1 lahat, dito hindi
        Zload(0) = New Complex(Resistance(0), Reactance(0))
        Zload(1) = New Complex(Resistance(1), Reactance(1))
        Zload(2) = New Complex(Resistance(2), Reactance(2))

        'for vline, gumawa ako ng new array here, nasa class
        VlineArray(0) = Complex.FromPolarCoordinates(Vline, Math.PI * 0 / 180)
        VlineArray(1) = Complex.FromPolarCoordinates(Vline, Math.PI * 240 / 180)
        VlineArray(2) = Complex.FromPolarCoordinates(Vline, Math.PI * 120 / 180)

        Dim Zx As Complex = (Zload(0) * Zload(1)) + (Zload(0) * Zload(2)) + (Zload(1) * Zload(2))

        'for iphase, di ko maisip pano i-for next so ganto nalang
        Iphase(0) = (VlineArray(0) * Zload(2) - VlineArray(2) * Zload(1)) / Zx
        Iphase(1) = (VlineArray(1) * Zload(0) - VlineArray(0) * Zload(2)) / Zx
        Iphase(2) = (VlineArray(2) * Zload(1) - VlineArray(1) * Zload(0)) / Zx

        'for vphase, just in case need
        For i As Integer = 0 To 2
            Vphase(i) = Iphase(i) * Zload(i)
        Next

        'for S
        For i As Integer = 0 To 2
            ApparentPower += (Vphase(i) * Complex.Conjugate(Iphase(i)))
        Next

        RealPower = Math.Round(ApparentPower.Real, 2)
        ReactivePower = Math.Round(ApparentPower.Imaginary, 2)
        PowerFactor = Math.Round(RealPower / ApparentPower.Magnitude, 2)

    End Sub
    Private Sub Wye_balanced()
        'declaring variable
        Zload(0) = New Complex(Resistance(0), Reactance(0))
        Zload(1) = New Complex(Resistance(0), Reactance(0))
        Zload(2) = New Complex(Resistance(0), Reactance(0))

        'for vphase = Vl sqrt3
        For i As Integer = 0 To 2
            Vphase(i) = Complex.FromPolarCoordinates(Vline / Math.Sqrt(3), Math.PI * (330 - (i * 120)) / 180)
        Next

        'for iphase
        For i As Integer = 0 To 2
            Iphase(i) = Complex.Divide(Vphase(i), Zload(i))
        Next

        'for power parameters
        ApparentPower = 3 * Complex.Multiply(Vphase(0), Complex.Conjugate(Iphase(0)))
        RealPower = Math.Round(ApparentPower.Real, 2)
        ReactivePower = Math.Round(ApparentPower.Imaginary, 2)
        PowerFactor = Math.Round(RealPower / ApparentPower.Magnitude, 2)


    End Sub
    Private Sub output() 'output subprocedure
        Try
            If ReactivePower < 0 Then
                Dim strimg As String = ReactivePower
                txtAppPower.Text = RealPower & " " & " - j" & strimg.Substring(1)
            Else
                txtAppPower.Text = RealPower & " + j" & ReactivePower
            End If

            Dim strIphImg(2) As String
            Dim strIph(2) As String
            For i As Integer = 0 To 2
                If Iphase(i).Imaginary < 0 Then
                    strIphImg(i) = Math.Round(Iphase(i).Imaginary, 2)
                    strIph(i) = Math.Round(Iphase(i).Real, 2) & " " & " - j" & strIphImg(i).Substring(1)
                Else
                    strIph(i) = Math.Round(Iphase(i).Real, 2) & " + j" & Math.Round(Iphase(i).Imaginary, 2)
                End If
            Next
            txtPhaseA.Text = strIph(0)
            txtPhaseB.Text = strIph(1)
            txtPhaseC.Text = strIph(2)
            txtRealPower.Text = RealPower
            txtReacPower.Text = ReactivePower
            txtPF.Text = PowerFactor

            lblV1.Text = "V₁ = " & Math.Round(Complex.Abs(Vphase(0)), 2) & " < " & Math.Round(Vphase(0).Phase * 180 / Math.PI, 2) & "°"
            lblV2.Text = "V₂  = " & Math.Round(Complex.Abs(Vphase(1)), 2) & " < " & Math.Round(Vphase(1).Phase * 180 / Math.PI, 2) & "°"
            lblV3.Text = "V₃ = " & Math.Round(Complex.Abs(Vphase(2)), 2) & " < " & Math.Round(Vphase(2).Phase * 180 / Math.PI, 2) & "°"
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load, btnReset.Click, ResetToolStripMenuItem.Click
        'initial conditions: cleared arrays, textboxes, radio buttons, focus on vline
        Array.Clear(Zload, 0, 2)
        Array.Clear(Vphase, 0, 2)
        Array.Clear(Iphase, 0, 2)
        Array.Clear(VlineArray, 0, 2)
        Array.Clear(Resistance, 0, 2)
        Array.Clear(Reactance, 0, 2)
        txtR1.Clear()
        txtR2.Clear()
        txtR3.Clear()
        txtX1.Clear()
        txtX2.Clear()
        txtX3.Clear()
        txtVline.Clear()
        lblV1.Text = "V₁ = Vaᵩ < ɸa°"
        lblV2.Text = "V₂  = Vbᵩ < ɸb°"
        lblV3.Text = "V₃ = Vcᵩ < ɸc°"
        Me.Width = 519
        btnShow.Text = "&Show Graph >>>"
        rdbBalanced.Checked = False
        rdbUnbalanced.Checked = False
        txtPhaseA.Clear()
        txtPhaseB.Clear()
        txtPhaseC.Clear()
        txtRealPower.Clear()
        txtReacPower.Clear()
        txtAppPower.Clear()
        txtPF.Clear()
        txtVline.Focus()
        grpOutput.Enabled = False
        btnShow.Enabled = False
        blnPlot = False
        picDraw.Refresh()
    End Sub

    Private Sub wyepic_click() Handles PictureBox1.Click 'pag pinindot yung pic, mapipindot din yung rdb
        rdbWye.Checked = True
        Call Change()
    End Sub

    Private Sub deltapic_click() Handles PictureBox2.Click 'pag pinindot yung pic, mapipindot din yung rdb
        rdbDelta.Checked = True
        Call Change()
    End Sub

    Private Sub rdbalanced_click(sender As Object, e As EventArgs) Handles rdbBalanced.Click
        'disabled ang ibang impedance pagkapindot sa balanced
        txtR2.Enabled = False
        txtR3.Enabled = False
        txtX2.Enabled = False
        txtX3.Enabled = False
        Call Change()
    End Sub
    Private Sub rdbunbalanced_click(sender As Object, e As EventArgs) Handles rdbUnbalanced.Click
        'enabled ang ibang impedance pag pinindot
        txtR2.Enabled = True
        txtR3.Enabled = True
        txtX2.Enabled = True
        txtX3.Enabled = True
        Call Change()
    End Sub

    Private Sub zoomIn_Click(sender As Object, e As EventArgs) Handles zoomIn.Click
        zoomfac *= 2
        picDraw.Refresh()

    End Sub

    Private Sub btnShow_Click(sender As Object, e As EventArgs) Handles btnShow.Click
        Try
            If btnShow.Text = "&Show Graph >>>" Then
                Me.Width = 860
                btnShow.Text = "<<< H&ide Graph"
            Else
                Me.Width = 519
                btnShow.Text = "&Show Graph >>>"
            End If
            zoomfac = 1
            blnPlot = True
            Dim maxvalue As Double
            For i As Integer = 0 To 2
                If Complex.Abs(Vphase(i)) > maxvalue Then
                    maxvalue = Complex.Abs(Vphase(i))
                End If
            Next
            If picDraw.Height / 2 > maxvalue Then
                Do
                    zoomfac *= 2
                Loop While picDraw.Height / 2 > (zoomfac * maxvalue)
                zoomfac /= 2
            ElseIf picDraw.Height / 2 < maxvalue Then

                Do
                    zoomfac /= 2
                Loop While picDraw.Height / 2 < (zoomfac * maxvalue)
            Else
                zoomfac /= 2

            End If

            picDraw.Refresh()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub rdbWye_CheckedChanged(sender As Object, e As EventArgs) Handles rdbWye.CheckedChanged
        Call Change()
    End Sub

    Private Sub rdbDelta_CheckedChanged(sender As Object, e As EventArgs) Handles rdbDelta.CheckedChanged
        Call Change()
    End Sub

    Private Sub zoomOut_Click(sender As Object, e As EventArgs) Handles zoomOut.Click
        zoomfac /= 2
        picDraw.Refresh()
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        If MessageBox.Show("Do you really want to exit?", "Exit?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = vbYes Then Me.Close()
    End Sub

    Private Sub ABoutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ABoutToolStripMenuItem.Click
        Form2.Show()
    End Sub

    Private Sub picReset()
        blnPlot = False
        picDraw.Refresh()

    End Sub

    Private Sub ComplexNumberFormConverterToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ComplexNumberFormConverterToolStripMenuItem.Click
        Form3.Show()
    End Sub

    Private Sub DrawAxes(ByVal g As Graphics)
        Try
            intXOrigin = picDraw.Width \ 2 'For Finding the center of the screen
            intYOrigin = picDraw.Height \ 2

            Dim myfont As New Font("Arial", 8, FontStyle.Regular)
            g.Clear(Color.Black)
            g.DrawLine(Pens.Gray, 0, intYOrigin, picDraw.Width, intYOrigin) 'Y-axis
            g.DrawLine(Pens.Gray, intXOrigin, 0, intXOrigin, picDraw.Height) 'X-axis
            g.DrawString("0,0", myfont, Brushes.Green, intXOrigin, intYOrigin) 'Label for Origin
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub
    Private Sub picDraw_Paint(sender As Object, e As PaintEventArgs) Handles picDraw.Paint
        Try
            Dim gPaint As Graphics = e.Graphics
            Select Case True
                Case blnPlot

                    Call DrawAxes(gPaint) 'Calling the axes
                    Dim sngStep As Single = 0.2
                    Dim phase(2)

                    Dim sngY As Single
                    For j As Integer = 0 To 2
                        phase(j) = Vphase(j).Phase


                        For counter As Single = Complex.Abs(Vphase(j)) * zoomfac To picDraw.Height Step CSng(Complex.Abs(Vphase(j))) * zoomfac 'Block of code for the markers and labels in Y-axis
                            gPaint.DrawLine(Pens.White, intXOrigin - 5, intYOrigin + CSng(counter), intXOrigin + 5, intYOrigin + CSng(counter))
                            gPaint.DrawLine(Pens.White, intXOrigin - 5, picDraw.Height - intYOrigin - CSng(counter), intXOrigin + 5, picDraw.Height - intYOrigin - CSng(counter))
                            gPaint.DrawString(counter / zoomfac, myfont, Brushes.Green, intXOrigin + 10, intYOrigin + CSng((counter - 5)))
                            gPaint.DrawString(counter / zoomfac, myfont, Brushes.Green, intXOrigin + 10, intYOrigin - CSng((counter - 5)))
                        Next

                        For counter As Single = 15 To picDraw.Width Step 15  'Block of code for the markers in the X-axis
                            gPaint.DrawLine(Pens.White, intXOrigin + CSng(counter), intYOrigin + 5, intXOrigin + CSng(counter), intYOrigin - 5)
                            gPaint.DrawLine(Pens.White, picDraw.Width - intXOrigin - CSng(counter), intYOrigin + 5, picDraw.Width - intXOrigin - CSng(counter), intYOrigin - 5)
                        Next
                        For counter As Single = 45 To picDraw.Width Step 45  'Block of  code  for the labels in the X-axis
                            gPaint.DrawString(counter, myfont, Brushes.Green, intXOrigin + CSng((counter) - 10), intYOrigin + 10)
                            gPaint.DrawString(counter, myfont, Brushes.Green, intXOrigin - CSng(counter + 10), intYOrigin + 10)
                        Next
                    Next

                    For sngX As Single = 0 To (picDraw.Width / 2) Step sngStep 'Block of code for the sine wave

                        sngY = Complex.Abs(Vphase(0)) * zoomfac * Math.Sin(sngX * Math.PI / 180 + phase(0))
                        Dim sngY2 As Single = Complex.Abs(Vphase(0)) * zoomfac * Math.Sin(sngX * Math.PI / 180 - phase(0))
                        gPaint.DrawRectangle(Pens.Coral, intXOrigin + sngX, intYOrigin - sngY, 1, 1)
                        gPaint.DrawRectangle(Pens.Coral, intXOrigin - sngX, intYOrigin + sngY2, 1, 1)

                    Next
                    For sngX As Single = 0 To (picDraw.Width / 2) Step sngStep 'Block of code for the sine wave

                        sngY = Complex.Abs(Vphase(1)) * zoomfac * Math.Sin(sngX * Math.PI / 180 + phase(1))
                        Dim sngY2 As Single = Complex.Abs(Vphase(1)) * zoomfac * Math.Sin(sngX * Math.PI / 180 - phase(1))

                        gPaint.DrawRectangle(Pens.Cyan, intXOrigin + sngX, intYOrigin - sngY, 1, 1)
                        gPaint.DrawRectangle(Pens.Cyan, intXOrigin - sngX, intYOrigin + sngY2, 1, 1)

                    Next

                    For sngX As Single = 0 To (picDraw.Width / 2) Step sngStep 'Block of code for the sine wave

                        sngY = Complex.Abs(Vphase(2)) * zoomfac * Math.Sin(sngX * Math.PI / 180 + phase(2))
                        Dim sngY2 As Single = Complex.Abs(Vphase(2)) * zoomfac * Math.Sin(sngX * Math.PI / 180 - phase(2))
                        gPaint.DrawRectangle(Pens.DarkOliveGreen, intXOrigin + sngX, intYOrigin - sngY, 1, 1)
                        gPaint.DrawRectangle(Pens.DarkOliveGreen, intXOrigin - sngX, intYOrigin + sngY2, 1, 1)

                    Next

                Case Else 'When the user input was not accepted by the program due to input error
                    gPaint.Clear(Color.Black)
                    Call DrawAxes(gPaint)

            End Select
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub
    Private Sub Form1_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Me.KeyPress
        If AscW(e.KeyChar) > 47 And AscW(e.KeyChar) < 58 Or AscW(e.KeyChar) = 8 Or AscW(e.KeyChar) = 46 Or AscW(e.KeyChar) = 45 Then
        Else
            e.KeyChar = Nothing
        End If
    End Sub

    Sub Change()
        Array.Clear(Zload, 0, 2)
        Array.Clear(Vphase, 0, 2)
        Array.Clear(Iphase, 0, 2)
        Array.Clear(VlineArray, 0, 2)
        Array.Clear(Resistance, 0, 2)
        Array.Clear(Reactance, 0, 2)
        lblV1.Text = "V₁ = Vaᵩ < ɸa°"
        lblV2.Text = "V₂  = Vbᵩ < ɸb°"
        lblV3.Text = "V₃ = Vcᵩ < ɸc°"
        Me.Width = 519
        btnShow.Text = "&Show Graph >>>"
        txtPhaseA.Clear()
        txtPhaseB.Clear()
        txtPhaseC.Clear()
        txtRealPower.Clear()
        txtReacPower.Clear()
        txtAppPower.Clear()
        txtPF.Clear()
        txtVline.Focus()
        grpOutput.Enabled = False
        btnShow.Enabled = False
        blnPlot = False
        picDraw.Refresh()
    End Sub
End Class

