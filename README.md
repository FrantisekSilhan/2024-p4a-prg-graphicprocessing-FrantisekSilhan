# Paralelizovan� zpracov�n� obrazu

V tomto projektu se nach�z� MAUI aplikace s uk�zkov�m k�dem pro paralelizovan� zpracov�n� obrazu. Aplikace je naps�na v jazyce C# a vyu��v� knihovny SkiaSharp a SkiaSharp.Extended pro pr�ci s obr�zky. Aktu�ln� podporuje t�i zp�soby zpracov�n� obrazu zam��en� na extrakci �erven�ho kan�lu:

1. Sekven�n� synchronn� zpracov�n�.
2. Sekven�n� synchronn� zpracov�n� pomoc� unsafe k�du.
3. Paraleln� zpracov�n� pomoc� Task.

Va��m �kolem je d�le roz���it funkcionalitu aplikace a prov�st vylep�en� podle n�e uveden�ch bod�.

## �koly k dopln�n� k�du

1. Implementujte paraleln� zpracov�n� obr�zku pomoc� Parallel.For bez pou�it� unsafe k�du.
1. Implementujte paraleln� zpracov�n� obr�zku pomoc� Parallel.For s pou�it�m unsafe k�du.
1. Navrhn�te a realizujte lep�� u�ivatelsk� rozhran� pro aplikaci (nap�. p�id�n� mo�nost� v�b�ru filtr�, indik�tor pr�b�hu, tla��tko pro zastaven� operace).
1. (Voliteln�) Refaktorujte aplikaci pro architekturu MVVM, aby byl k�d p�ehledn�j�� a snadno roz�i�iteln�.

## Vlastn� filtr

Vytvo�te vlastn� efektivn� paraleln� filtry pro zpracov�n� obr�zku. V�sledek zobrazte v aplikaci. Pokud budou V� filry vy�adovat dal�� ovl�dac� prvky v u�ivatelsk�m rozhran�, dopl�te je a popi�te jejich funk�nost. 
Vyberte si jeden jednoduch� (ozna�en� hv�zdi�kou) a jeden slo�it�j��.

## Popis vybran�ch algoritm�

### Zv�razn�n� hran
- **Jak funguje**: Detekuje hrany na z�klad� zm�n jasu nebo barev v obraze. Nej�ast�ji se pou��v� Sobel�v oper�tor, Cannyho detekce hran nebo Laplace�v oper�tor.
- **Princip**: Aplikuje se konvolu�n� filtr s matic� detekuj�c� zm�ny intenzity v x a y sm�ru.
- **Odkaz**: [Sobel a Laplaceovy filtry na Wikipedia](https://en.wikipedia.org/wiki/Sobel_operator)

### Zv�razn�n� oblast� s vysok�m kontrastem
- **Jak funguje**: Vyhled�v� oblasti s rychl�mi zm�nami intenzity mezi sousedn�mi pixely a zv�raz�uje je.
- **Princip**: Aplikace vysokofrekven�n�ho filtru (nap�. pomoc� Fourierovy transformace).
- **Odkaz**: [Fourierova anal�za obrazu](https://en.wikipedia.org/wiki/Fourier_transform)

### Zv�razn�n� oblast� s n�zk�m kontrastem 
- **Jak funguje**: Naopak, hled� oblasti s pomal�mi zm�nami intenzity a zvy�uje jejich viditelnost.
- **Princip**: Pou�it� n�zkofrekven�n�ho filtru na odstran�n� vysok�ch detail�.
- **Odkaz**: [Gaussian Blur na Wikipedia](https://en.wikipedia.org/wiki/Gaussian_blur)

### Zv�razn�n� oblast� s vysok�m jasem (*)
- **Jak funguje**: Zv�raz�uje pixely, jejich� jas (luminance) p�ekra�uje ur�itou mez.
- **Princip**: Luminance je obvykle vypo��t�na jako v�en� kombinace RGB hodnot: \( Y = 0.2126R + 0.7152G + 0.0722B \).
- **Odkaz**: [Luminance a barvy](https://en.wikipedia.org/wiki/Relative_luminance)

### Zv�razn�n� oblast� s n�zk�m jasem (*)
- **Jak funguje**: Zam��uje se na pixely s n�zkou luminanc�.
- **Princip**: Podobn� jako u vysok�ho jasu, ale se zam��en�m na doln� prahovou hodnotu.
- **Odkaz**: Viz luminance v��e.

### Zv�razn�n� oblast� s vysokou sytost� (*)
- **Jak funguje**: Identifikuje oblasti s intenzivn� barvou (saturace) a zv�razn� je.
- **Princip**: Saturace je vypo��t�na v HSV nebo HSL modelu. V HSV se saturace ur�uje jako \( S = \frac{max - min}{max} \).
- **Odkaz**: [HSV model na Wikipedia](https://en.wikipedia.org/wiki/HSL_and_HSV)

### Zv�razn�n� oblast� s n�zkou sytost� (*)
- **Jak funguje**: Zam��uje se na oblasti s n�zkou saturac�, kter� jsou t�m�� �ed�.
- **Princip**: Filtrace barev se saturac� pod ur�itou hranici.
- **Odkaz**: Viz HSV model v��e.

### P�evod na �ernob�l� obr�zek nastaven�m prahu jasu (*)
- **Jak funguje**: Ka�d� pixel se porovn� s prahovou hodnotou. Pokud je vy���, pixel je b�l�, jinak �ern�.
- **Princip**: \( grayscale > threshold ? white : black \).
- **Odkaz**: [Binarizace na Wikipedia](https://en.wikipedia.org/wiki/Thresholding_(image_processing))

### P�evod na �ernob�l� obr�zek pomoc� v�po�tu brightness (*)
- **Jak funguje**: Brightness (jas) je jednoduch� pr�m�r RGB kan�l�: \( B = \frac{R + G + B}{3} \).
- **Princip**: Tento p��stup m��e b�t m�n� p�esn� ne� luminance.
- **Odkaz**: Viz z�kladn� barvy v��e.

### P�evod na �ernob�l� obr�zek pomoc� v�po�tu luminance (*)
- **Jak funguje**: P�esn�j�� ne� brightness, pou��v� v�en� pr�m�r RGB hodnot (viz zv�razn�n� vysok�ho jasu).
- **Odkaz**: [Luminance](https://en.wikipedia.org/wiki/Relative_luminance)

### Rozmaz�n� obr�zku pomoc� pr�m�rov�n� pixel� v dan�m segmentu
- **Jak funguje**: Pro ka�d� pixel se vypo��t� pr�m�r hodnot v okoln�m okn�.
- **Princip**: Aplikuje se konvolu�n� filtr s pr�m�rovac� maskou (box filter).
- **Odkaz**: [Box blur](https://en.wikipedia.org/wiki/Box_blur)

### Rozmaz�n� obr�zku pomoc� medi�nov�ho filtru
- **Jak funguje**: Nam�sto pr�m�ru se v okn� vybere medi�n hodnot.
- **Princip**: Efektivn� na odstran�n� �umu (nap�. s�l a pep�).
- **Odkaz**: [Median filter](https://en.wikipedia.org/wiki/Median_filter)

### Invertov�n� barev � P�evr�cen� barevn�ch hodnot (*)
- **Jak funguje**: Ka�d� pixel \( (R, G, B) \) je p�eveden na \( (255-R, 255-G, 255-B) \).
- **Odkaz**: [Color inversion](https://en.wikipedia.org/wiki/Color_negative)

### Zm�na sytosti � Zv��en� nebo sn�en� sytosti barev podle nastaven�
- **Jak funguje**: Modifikuje saturaci v HSV nebo HSL barevn�m modelu. Zv��en� sytosti zintenzivn� barvy, sn�en� je p�ibli�uje �ed�.
- **Princip**: Sytost se po��t� jako \( S = \frac{max - min}{max} \). Zm�na sytosti zahrnuje p�en�soben� t�to hodnoty koeficientem.
- **Odkaz**: [HSV model na Wikipedia](https://en.wikipedia.org/wiki/HSL_and_HSV)

### Zm�na kontrastu (*) podle nastaven�
- **Jak funguje**: Zv��en� kontrastu roz���� rozd�ly mezi tmav�mi a sv�tl�mi oblastmi, sn�en� kontrastu tyto rozd�ly zmen��.
- **Princip**: Aplikuje se transformace na pixelov� hodnoty, nap�. \( newValue = (oldValue - 128) \times factor + 128 \).
- **Odkaz**: [Kontrast v obrazech](https://en.wikipedia.org/wiki/Contrast_(vision))

### Zv�razn�n� barevn�ho odst�nu (*)
- **Jak funguje**: Zv�raz�uje nebo sni�uje intenzitu ur�it�ho odst�nu v barevn�m spektru.
- **Princip**: V HSV modelu se uprav� pouze H (hue), ostatn� hodnoty (sytost a jas) z�st�vaj� nezm�n�n�.
- **Odkaz**: Viz [HSV model](https://en.wikipedia.org/wiki/HSL_and_HSV)

### Gamma korekce (*)
- **Jak funguje**: Zm�n� jas obrazu pomoc� exponenti�ln� transformace. Pou��v� se pro kompenzaci nelinearity lidsk�ho vn�m�n�.
- **Princip**: Transformace pixel� podle \( newValue = 255 \times \left(\frac{oldValue}{255}\right)^\gamma \).
- **Odkaz**: [Gamma korekce](https://en.wikipedia.org/wiki/Gamma_correction)

### Selektivn� barevnost (*) s v�b�rem odst�nu
- **Jak funguje**: Zachov� jednu barvu (nap�. �ervenou) a odbarv� ostatn� ��sti obrazu.
- **Princip**: Pou��v� se maska pro zachov�n� barev v ur�it�m rozsahu odst�n�, ostatn� barvy se p�evedou na stupn� �edi.
- **Odkaz**: [Selective colorization tutorial](https://docs.gimp.org/en/)

### Posterizace (*?)
- **Jak funguje**: Sni�uje po�et barev v obrazu, co� vytv��� efekt s ostr�mi p�echody.
- **Princip**: Pixelov� hodnoty se zaokrouhluj� na nejbli��� hodnotu v omezen� palet�.
- **Odkaz**: [Posterization](https://en.wikipedia.org/wiki/Posterization)

### Pixelace
- **Jak funguje**: Obr�zek se rozd�l� na v�t�� bloky (pixely), kter� maj� stejnou barvu.
- **Princip**: Pro ka�d� blok se vypo��t� pr�m�rn� barva a aplikuje se na cel� blok.
- **Odkaz**: [Pixelation](https://en.wikipedia.org/wiki/Pixelation)

### Sepia efekt (*)
- **Jak funguje**: P�evede obraz do hn�d�ch t�n�, co� vytv��� starobyl� vzhled.
- **Princip**: Kombinuje RGB kan�ly s v�en�mi koeficienty: 
  \( R' = 0.393R + 0.769G + 0.189B \), 
  \( G' = 0.349R + 0.686G + 0.168B \), 
  \( B' = 0.272R + 0.534G + 0.131B \).
- **Odkaz**: [Sepia toning](https://en.wikipedia.org/wiki/Sepia_toning)

### Vignette efekt
- **Jak funguje**: Ztmavuje okraje obrazu a zv�raz�uje st�ed.
- **Princip**: Intenzita pixel� na okraj�ch je zmen�ena podle jejich vzd�lenosti od st�edu obrazu.
- **Odkaz**: [Vignette efekt](https://en.wikipedia.org/wiki/Vignetting)

### P�id�n� �umu (*)
- **Jak funguje**: P�id�v� n�hodn� hodnoty k pixel�m, aby se simuloval �um (nap�. �um kamery).
- **Princip**: Ke ka�d�mu pixelu se p�i�te n�hodn� hodnota z ur�it�ho rozsahu.
- **Odkaz**: [Noise in images](https://en.wikipedia.org/wiki/Image_noise)

### Odstran�n� �umu
- **Jak funguje**: Pou��v� filtraci (nap�. pr�m�rov�n� nebo gaussovsk� rozmaz�n�), aby odstranil ne��douc� �um.
- **Princip**: Nahrazen� hodnoty pixelu pr�m�rem hodnot z jeho okol�.
- **Odkaz**: [Noise reduction](https://en.wikipedia.org/wiki/Noise_reduction)

### Embossing
- **Jak funguje**: Vytv��� reli�fn� vzhled obrazu simulac� 3D efektu.
- **Princip**: Aplikuje se konvolu�n� filtr s matic� zv�raz�uj�c� p�echody mezi sousedn�mi pixely.
- **Odkaz**: [Embossing filter](https://en.wikipedia.org/wiki/Image_embossing)

### Sharpening
- **Jak funguje**: Zv�raz�uje hrany v obrazu, aby vypadal ost�eji.
- **Princip**: Kombinuje p�vodn� obraz s jeho zv�razn�n�mi hranami.
- **Odkaz**: [Image sharpening](https://en.wikipedia.org/wiki/Unsharp_masking)

### Detekce barevn�ch oblast� (*) s v�b�rem barvy
- **Jak funguje**: Zv�raz�uje oblasti s ur�itou dominantn� barvou (nap�. zelen� listy).
- **Princip**: Barevn� oblasti se identifikuj� podle HSV modelu.
- **Odkaz**: [Color detection in HSV](https://en.wikipedia.org/wiki/HSL_and_HSV)

### Rotace barevn�ho spektra (*)
- **Jak funguje**: Posouv� barvy v HSV modelu (nap�. �erven� se zm�n� na modrou).
- **Princip**: Hodnota H (hue) se p�i�te nebo ode�te o ur�itou konstantu.
- **Odkaz**: Viz HSV model v��e.

### Kaleidoskopick� efekt (* * *)
- **Jak funguje**: Zrcadl� ��sti obrazu podle os symetrie.
- **Princip**: Pixely jsou p�emapov�ny podle pravidel symetrie.
- **Odkaz**: [Kaleidoscopic effect tutorial](https://docs.gimp.org/en/)

### Skl�d�n� kan�l� (*)
- **Jak funguje**: Z�m�na barevn�ch kan�l� mezi sebou (nap�. �erven� za zelen�).
- **Princip**: Manipulace s hodnotami R, G a B v ka�d�m pixelu.
- **Odkaz**: [Channel swapping in images](https://en.wikipedia.org/wiki/RGB_color_model)

### Vyhlazen�
- **Jak funguje**: Zm�k�uje obraz odstran�n�m vysokofrekven�n�ch detail�.
- **Princip**: Aplikuje se gaussovsk� nebo boxov� filtr.
- **Odkaz**: [Smoothing filters](https://en.wikipedia.org/wiki/Gaussian_blur)

### Binarizace (*)
- **Jak funguje**: P�ev�d� obraz na �ernob�l� podle nastaven�ho prahu.
- **Princip**: Pokud je hodnota pixelu nad prahem, je nastavena na b�lou, jinak na �ernou.
- **Odkaz**: Viz [Thresholding](https://en.wikipedia.org/wiki/Thresholding_(image_processing))

### Kreslen� kontur
- **Jak funguje**: Zv�raz�uje linie v obrazu podle p�echod� jasu.
- **Princip**: Aplikuje se Laplace�v nebo Sobel�v filtr.
- **Odkaz**: [Edge detection](https://en.wikipedia.org/wiki/Edge_detection)

### Zv�razn�n� textury
- **Jak funguje**: Zv�raz�uje drobn� detaily a hrubosti povrchu.
- **Princip**: Aplikuje se vysokofrekven�n� filtr.
- **Odkaz**: [Texture enhancement](https://en.wikipedia.org/wiki/Image_texture)

### Barevn� rozd�len� (* * *)
- **Jak funguje**: Rozd�luje obraz na oblasti podle barevn�ch segment�.
- **Princip**: Pou��v� se k-means clustering nebo podobn� metoda.
- **Odkaz**: [Color segmentation](https://en.wikipedia.org/wiki/Image_segmentation)

### Gradientn� t�nov�n�
- **Jak funguje**: P�id�v� p�es obraz barevn� gradient.
- **Princip**: P�i��taj� se barevn� hodnoty podle polohy pixelu v obraze.
- **Odkaz**: [Gradient overlay tutorial](https://docs.gimp.org/en/)

### Simulace barevn� slepoty
- **Jak funguje**: Zm�n� barevn� spektrum tak, aby simulovalo ur�it� typ barevn� slepoty (nap�. deuteranopie).
- **Princip**: Barevn� kan�ly jsou transformov�ny podle p��slu�n�ho modelu slepoty.
- **Odkaz**: [Color blindness simulation](https://en.wikipedia.org/wiki/Color_blindness)

### Prol�n�n� dvou obr�zk� (*?) s v�b�rem m�ry prol�n�n�
- **Jak funguje**: Opticky zpr�m�ruje hodnoty pixel� z dvou obr�zk�.
- **Princip**: Pro ka�d� pixel se vypo��t� v�en� pr�m�r hodnot z obou obr�zk�.
- **Odkaz**: [Cross-fading tutorial](https://docs.gimp.org/en/)

### Pohybov� rozmaz�n� dle dan�ho vektoru
- **Jak funguje**: Simuluje pohyb kamery nebo objektu v obraze.
- **Princip**: Pixelov� hodnoty se pr�m�ruj� podle sm�ru pohybu.
- **Odkaz**: [Motion blur](https://en.wikipedia.org/wiki/Motion_blur)

### Pl�tno s texturou
- **Jak funguje**: P�id�v� texturu na obraz, aby vypadal jako malba na pl�tn�.
- **Princip**: Textura se p�ekr�v� s obrazem a m��e b�t pr�hledn�.
- **Odkaz**: [Canvas texture tutorial](https://docs.gimp.org/en/)

### Vlastn� v�po�et algoritmu HOG (* * *)
- **Jak funguje**: Vytv��� histogram orientovan�ch gradient� pro detekci objekt�.
- **Princip**: Pro ka�d� blok o dan� velikosti se vypo��t� gradient a jeho orientace, kter� se seskupuj� do histogram�.
- **Odkaz**: [Histogram of oriented gradients](https://en.wikipedia.org/wiki/Histogram_of_oriented_gradients)

### Jak�koli jin� vlastn� filtr
- **Inspirace**: GIMP, Photopea, Photoshop, Lightroom, ...

Pozn�mka: P�i n�vrhu filtru dbejte na efektivn� vyu�it� paraleln�ho zpracov�n�.