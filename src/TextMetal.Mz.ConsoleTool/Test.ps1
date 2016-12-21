cls

for ($i = 0; $i -lt 1000; $i++)
{	
	Copy-Item "2ndAsset.MessagingEngine.exe.config" ("d:\#dev_temp\_in_drop_box_\" + $i.ToString("0000") + "_2ndAsset.MessagingEngine.exe.config")
}