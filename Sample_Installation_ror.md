INSTALL DEPENDENCIES
sudo apt install curl
curl -sL https://deb.nodesource.com/setup_12.x | sudo -E bash -
curl -sS https://dl.yarnpkg.com/debian/pubkey.gpg | sudo apt-key add -
echo "deb https://dl.yarnpkg.com/debian/ stable main" | sudo tee /etc/apt/sources.list.d/yarn.list

sudo apt-get update
sudo apt-get install git-core zlib1g-dev build-essential libssl-dev libreadline-dev libyaml-dev libsqlite3-dev sqlite3 libxml2-dev libxslt1-dev libcurl4-openssl-dev software-properties-common libffi-dev nodejs yarn
INSTALL RUBY USING RVM
sudo apt-get install libgdbm-dev libncurses5-dev automake libtool bison libffi-dev
gpg --keyserver hkp://keys.gnupg.net --recv-keys 409B6B1796C275462A1703113804BB82D39DC0E3 7D2BAF1CF37B13E2069D6956105BD0E739499BDB
curl -sSL https://get.rvm.io | bash -s stable
source ~/.rvm/scripts/rvm
rvm install 2.6.5
rvm use 2.6.5 --default
ruby -v
INSTALL BUNDLER
gem install bundler
INSTALL RAILS 6.0.3
gem install rails -v 6.0.3
CONFIGURE GIT
git config --global color.ui true
git config --global user.name "YOUR NAME"
git config --global user.email "YOUR@EMAIL.com"
ssh-keygen -t rsa -b 4096 -C "YOUR@EMAIL.com"

cat ~/.ssh/id_rsa.pub # COPY AND PASTE THE RESULT TO https://github.com/settings/ssh/new
ssh -T git@github.com # TEST CONNECTION
PULL PROJECT FROM GITHUB
git clone git@github.com:motorme/KeepSafe-Rails.git

cd Keepsafe-Rails
sudo nano ~/.bash_profile
# PASTE AT THE BOTTOM AND SAVE FILE
export RAILS_SMS_ENABLED=true
export RAILS_ASSETS_URL=apps.keepsafe.ph
export RAILS_ASSETS_PROTOCOL=https
ADD THE MASTER KEY
sudo nano config/master.key

# PASTE THIS AND SAVE FILE (CANNOT BE REPLACED)
9997d0da93d1ca32917f112d80f236de
INSTALL AND CONFIGURE NGINX
sudo apt-get install nginx
sudo service nginx start
cd /etc/nginx/sites-enabled
sudo rm default
sudo ln -s /home/ubuntu/KeepSafe-Rails/config/keepsafe.ph keepsafe.ph
sudo service nginx restart
cd ~/Keepsafe-Rails/
bundle
cd /etc/nginx/sites-enabled
sudo chown -R www-data:www-data /home/ubuntu/KeepSafe-Rails/public
chmod 701 /home/ubuntu/
sudo service nginx restart
DEPLOY APPLICATION
cd ~/Keepsafe-Rails/
# First unzip the assets.zip and image_creator.zip files into public folder

bundle
sudo cp app/assets/images/* public/assets
rake assets:precompile
source ~/.bash_profile
ruby sidekiq.rb stop
rake db:migrate RAILS_ENV=production

sudo kill -9 `cat tmp/pids/puma.pid`
ruby sidekiq.rb start
bundle exec puma -C /home/ubuntu/KeepSafe-Rails/config/production_puma.rb -d
crontab -r
whenever -w
tail -f log/production.log -n 1000
FOR DATABASE (NOT NEEDED IF USING AWS DATABASE)
sudo apt update
sudo apt install postgresql postgresql-contrib libpq-dev

sudo su - postgres
psql

CREATE ROLE ubuntu CREATEROLE CREATEDB LOGIN SUPERUSER;
\password ubuntu

CREATE DATABASE keepsafe_server_production OWNER ubuntu;
BACK UP DATABASE FROM SERVER
pg_dump -U ubuntu keepsafe_server_production -h localhost -F t > keepsafe_production_db.dump
# NEED DATABASE PASSWORD
RESTORE DATABASE FROM SERVER
pg_restore --verbose --clean --no-acl --no-owner -h localhost -U ubuntu -d CHANGE_THIS_TO_YOUR_DATABASE_NAME keepsafe_production_db.dump
# NEED DATABASE PASSWORD
CHANGE RAILS DATABASE CONNECTION
# WONT WORK IF MASTER.KEY IS NOT ADDED
EDITOR=nano rails credentials:edit