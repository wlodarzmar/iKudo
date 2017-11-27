rd /s /q Kudo.git
git clone --bare https://mwlodarz.visualstudio.com/DefaultCollection/_git/Kudo
cd Kudo.git
git remote add --mirror=fetch kudogithub https://github.com/wlodarzmar/iKudo.git
git fetch origin --all
git push kudogithub --all

