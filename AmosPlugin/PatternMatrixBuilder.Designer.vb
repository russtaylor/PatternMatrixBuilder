<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PatternMatrixBuilder
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.MatrixInput = New System.Windows.Forms.TextBox()
        Me.ProcessButton = New System.Windows.Forms.Button()
        Me.CancelButtonControl = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'MatrixInput
        '
        Me.MatrixInput.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.MatrixInput.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MatrixInput.Location = New System.Drawing.Point(13, 27)
        Me.MatrixInput.Multiline = True
        Me.MatrixInput.Name = "MatrixInput"
        Me.MatrixInput.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.MatrixInput.Size = New System.Drawing.Size(744, 500)
        Me.MatrixInput.TabIndex = 0
        '
        'ProcessButton
        '
        Me.ProcessButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ProcessButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ProcessButton.Location = New System.Drawing.Point(639, 533)
        Me.ProcessButton.Name = "ProcessButton"
        Me.ProcessButton.Size = New System.Drawing.Size(118, 23)
        Me.ProcessButton.TabIndex = 1
        Me.ProcessButton.Text = "Create Diagram"
        Me.ProcessButton.UseVisualStyleBackColor = True
        '
        'CancelButtonControl
        '
        Me.CancelButtonControl.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CancelButtonControl.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.CancelButtonControl.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CancelButtonControl.Location = New System.Drawing.Point(558, 533)
        Me.CancelButtonControl.Name = "CancelButtonControl"
        Me.CancelButtonControl.Size = New System.Drawing.Size(75, 23)
        Me.CancelButtonControl.TabIndex = 2
        Me.CancelButtonControl.Text = "Cancel"
        Me.CancelButtonControl.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(387, 15)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Paste the copied data from your SPSS pattern matrix in the box below."
        '
        'PatternMatrixBuilder
        '
        Me.AcceptButton = Me.ProcessButton
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.CancelButtonControl
        Me.ClientSize = New System.Drawing.Size(769, 568)
        Me.ControlBox = False
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.CancelButtonControl)
        Me.Controls.Add(Me.ProcessButton)
        Me.Controls.Add(Me.MatrixInput)
        Me.MinimizeBox = False
        Me.Name = "PatternMatrixBuilder"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.Text = "Pattern Matrix Input"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents MatrixInput As System.Windows.Forms.TextBox
    Friend WithEvents ProcessButton As System.Windows.Forms.Button
    Friend WithEvents CancelButtonControl As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
End Class
