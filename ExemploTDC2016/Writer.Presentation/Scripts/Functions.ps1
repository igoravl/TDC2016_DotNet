Function New-RibbonGroup($Text)
{
    $group = New-Object 'System.Windows.Controls.Ribbon.RibbonGroup'
    $group.Header = $Text

    return $group
}

Function New-RibbonButton($Text, $Image, [scriptblock] $Action)
{
    $btn = New-Object 'System.Windows.Controls.Ribbon.RibbonButton'
    $btn.Label = $Text
    #$btn.LargeImage = $Image
    $btn.add_Click($Action)

    return $btn
}

Function Add-RibbonButton($Group, $Button, $Image, [scriptblock] $Action)
{
    $grp = ($App.RibbonGroups | ? Header -eq $Group)

    if (-not $grp)
    {
        $grp = New-RibbonGroup -Text $Group
    }

    $btn = New-RibbonButton -Text $Button -Image $Image -Action $Action

    $grp.Items.Add($btn)
    $App.RibbonGroups.Add($grp)
}

Function Add-Text($Document, $Text)
{
    $p = New-Object 'System.Windows.Documents.Paragraph'
    $r = New-Object 'System.Windows.Documents.Run' ($Text)

    $p.Inlines.Add($r)

    $Document.Blocks.Add($p)
}

Function Show-MessageBox($Text)
{
    [System.Windows.Forms.MessageBox]::Show($Text)
}

Function AutoExec
{
    Add-RibbonButton -Group 'Grupo 1' -Button 'Botao 1' -Action ({ Show-MessageBox 'Ribbon 1!'})
    Add-RibbonButton -Group 'Grupo 1' -Button 'Botao 2' -Action ({ Show-MessageBox 'Ribbon 2!'})
}
