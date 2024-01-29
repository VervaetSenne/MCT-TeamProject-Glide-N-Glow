# vars
ASPURL="http://10.10.10.13:5165"
SQLPASS="GNG_TeamProj2024"

# install mqtt
apt install mosquitto
touch /etc/mosquitto/mosquitto.conf > "listener 1883 0.0.0.0 \nallow_anonymous true"
echo "mosquitto install succesful"

# install mariadb
apt install mariadb-server
sudo mysql -e "CREATE DATABASE GlideNGlow"
sudo mysql -e "CREATE USER 'glidenglow_user'@'localhost' IDENTIFIED BY '$SQLPASS'"
sudo mysql -e "GRANT ALL PRIVILEGES ON GlideNGlow.* TO 'glidenglow_user'@'localhost' IDENTIFIED BY '$SQLPASS'"

echo "mariadb install succesful"

apt install apache2 -y
touch /etc/apache2/sites-available/000-default.conf > '
<VirtualHost *:80>
    ServerAdmin webmaster@localhost
    DocumentRoot /app/glidenglow-frontend        
    ErrorLog ${APACHE_LOG_DIR}/error.log
    CustomLog ${APACHE_LOG_DIR}/access.log combined
</VirtualHost>
'

cp ./GlideNGlow/GlideNGlow.Ui.WebApi/glidenglow-frontend /app/glidenglow-frontend

echo "apache install succesful"
echo "website copied succesful"
chmod 666 -R /app/glidenglow-frontend

# install dotnet
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x ./dotnet-install.sh
./dotnet-install.sh --version 7.0.405
export PATH="$PATH:/root/.dotnet"

echo "dotnet install successful"

# install efcore and update db
dotnet tool install --global dotnet-ef --version 7
export PATH="$PATH:~/.dotnet/tools"

echo "dotnet ef tool install successful"

dotnet ef database update --context GlideNGlowDbContext --startup-project ./GlideNGlow/GlideNGlow.Ui.WepApi/GlideNGlow.Ui.WepApi.csproj --configuration Release -- --environment Production

echo "database up to date"

# build project
dotnet build ./GlideNGlow/GlideNGlow.Ui.WepApi/GlideNGlow.Ui.WepApi.csproj --configuration Release --property:ASPNETCORE_URLS='http://10.10.10.13:5165' --output /app/
export ASPNETCORE_URLS=$ASPURL
export ASPNETCORE_ENVIRONMENT="Production"
chmod 666 /app/appsettings.json

echo "build successful"
echo "done."