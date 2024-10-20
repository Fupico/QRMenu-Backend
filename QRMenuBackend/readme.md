# Proje Çalıştırma

## Projeyi temizleme

`dotnet clean`

## Kütüphaneleri tekrar yükleme 

`dotnet restore`

## Projeyi Build Alma 

`dotnet build`

## Projenin Debug Modda Çalıştırılması

`dotnet run`



# Yeni Migration Ekleme, 


`DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 dotnet ef migrations add 2`



# FirstData Migration Güncelleme, 


`DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 dotnet ef database update FirstData`

# publish

 `dotnet publish -c Release -o ./publish`