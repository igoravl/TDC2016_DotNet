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
    $btn.LargeImageSource = (Get-ImageFromResource $Image)
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

Function Get-ImageFromResource($ImageName)
{
    $bmp = New-Object 'System.Windows.Media.Imaging.BitmapImage'
    $bmp.BeginInit()
    $bmp.UriSource = [uri] "pack://application:,,,/resources/images/$ImageName.png"
    $bmp.EndInit()

    return $bmp
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

Function New-FileDocument
{
    $App.FileService.NewCommand.Execute($null)
}

Function Open-FileDocument($file)
{
    $App.FileService.OpenCommand.Execute($file)
}

Function AutoExec
{
    Add-RibbonButton -Group 'Ferramentas' -Button 'Planilhas' -Image 'Document Spreadsheet' -Action ({ Show-MessageBox 'Abrir planilhas'})
    Add-RibbonButton -Group 'Mídias Sociais' -Button 'Facebook' -Image 'Social Facebook' -Action ({ Start-Process 'https://www.facebook.com/lambda3br' })
    Add-RibbonButton -Group 'Mídias Sociais' -Button 'Twitter' -Image 'Social Twitter' -Action ({ Start-Process 'https://twitter.com/lambdatres' })
}
