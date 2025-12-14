Public Class ucSBar
    Dim _minMaxVisible = True
    Dim _minValue = 0
    Dim _maxValue = 50
    'Dim _curValue = -1
    Dim _percentComplete = -1

    Dim _pulseWidthVal = -1

    Dim _xStart As Integer = 0
    Dim _xEnd As Integer = 0



    Public Property MinMaxVisible As Boolean
        Get
            Return _minMaxVisible
        End Get
        Set(value As Boolean)
            If _minMaxVisible <> value Then
                _minMaxVisible = value
                If _minMaxVisible = True Then
                    TableLayoutPanel1.Visible = True
                    Height = 15
                Else
                    TableLayoutPanel1.Visible = False
                    Height = 3
                End If
            End If
        End Set
    End Property

    Public Property MinValue As Integer
        Get
            Return _minValue
        End Get
        Set(value As Integer)
            If _minValue <> value Then
                _minValue = value
                _xStart = Map(_minValue, 0, 100, 0, pnlBackgroundBar.Width, False)

                If _minMaxVisible = True Then
                    If _minValue <= _maxValue Then
                        If lblMin.Text <> _minValue Then
                            lblMin.Text = _minValue
                        End If
                    Else
                        If lblMax.Text <> _minValue Then
                            lblMax.Text = _minValue
                        End If
                    End If


                End If
            End If
        End Set
    End Property
    Public Property MaxValue As Integer
        Get
            Return _maxValue
        End Get
        Set(value As Integer)
            If _maxValue <> value Then
                _maxValue = value
                _xEnd = Map(_maxValue, 0, 100, 0, pnlBackgroundBar.Width, False)

                If _minMaxVisible = True Then
                    If _minValue <= _maxValue Then
                        If lblMax.Text <> _maxValue Then
                            lblMax.Text = _maxValue
                        End If
                    Else
                        If lblMin.Text <> _maxValue Then
                            lblMin.Text = _maxValue
                        End If
                    End If
                End If
            End If
        End Set
    End Property
    Public Property PulseWidthCurVal As Integer
        'set this value prior to setting PercentComplete value.
        Get
            Return _pulseWidthVal
        End Get
        Set(value As Integer)
            _pulseWidthVal = value
        End Set
    End Property
    Public Property PercentComplete As Integer
        'Set the PulseWidthCurVal property prior to setting this property. Otherwise the lblPulseWidth label
        'will be set to stale data.
        Get
            Return _percentComplete
        End Get
        Set(newValue As Integer)
            _percentComplete = newValue
            Dim tmpWidth = 0
            Dim tmpLeft = 0
            Dim tmpRight = 0

            'Calculate the width of the progress bar:
            If _xStart <= _xEnd Then
                tmpLeft = _xStart
                tmpRight = _xEnd
            Else
                tmpLeft = _xEnd
                tmpRight = _xStart
            End If

            tmpWidth = Map(_percentComplete, 0, 100, 0, tmpRight - tmpLeft, True)

            'do we need to update the progress bar?
            If _xStart <= _xEnd Then
                If (lblForegroundBar.Left <> tmpLeft) Or (lblForegroundBar.Width <> tmpWidth) Then
                    lblForegroundBar.Left = tmpLeft
                    lblForegroundBar.Width = tmpWidth
                End If
            Else
                If (lblForegroundBar.Left <> (tmpRight - tmpWidth)) Or (lblForegroundBar.Width <> tmpWidth) Then
                    lblForegroundBar.Left = tmpRight - tmpWidth
                    lblForegroundBar.Width = tmpWidth
                End If
            End If
            Dim tmpMarkerPos = tmpLeft 'If((tmpLeft - 3) > 0, 0, tmpLeft - 3)
            If lblLeftMarker.Left <> tmpMarkerPos Then
                lblLeftMarker.Left = tmpMarkerPos
            End If
            tmpMarkerPos = If(tmpRight < pnlBackgroundBar.Width - 3, tmpRight, pnlBackgroundBar.Width - 3)
            If lblRightMarker.Left <> tmpMarkerPos Then
                lblRightMarker.Left = tmpMarkerPos
            End If

            'update the current pw value label
            If _minMaxVisible = True Then
                If lblCurVal.Text <> _pulseWidthVal Then
                    lblCurVal.Text = _pulseWidthVal
                End If
            End If

        End Set
    End Property


    Private Function Map(ByVal inVal As Integer, ByVal inMin As Integer, ByVal inMax As Integer,
                         ByVal outMin As Integer, ByVal outMax As Integer,
                         Optional ByVal clampLimits As Boolean = False) As Integer
        Dim retVal As Integer = (inVal - inMin) * (outMax - outMin) / (inMax - inMin) + outMin
        If clampLimits Then
            retVal = If(retVal < outMin, outMin, If(retVal > outMax, outMax, retVal))
        End If
        Return retVal
    End Function
End Class
