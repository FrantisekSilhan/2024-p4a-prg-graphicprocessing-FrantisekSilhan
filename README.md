# Paralelizované zpracování obrazu

V tomto projektu se nachází MAUI aplikace s ukázkovým kódem pro paralelizované zpracování obrazu. Aplikace je napsána v jazyce C# a využívá knihovny SkiaSharp a SkiaSharp.Extended pro práci s obrázky. Aktuálnì podporuje tøi zpùsoby zpracování obrazu zamìøené na extrakci èerveného kanálu:

1. Sekvenèní synchronní zpracování.
2. Sekvenèní synchronní zpracování pomocí unsafe kódu.
3. Paralelní zpracování pomocí Task.

Vaším úkolem je dále rozšíøit funkcionalitu aplikace a provést vylepšení podle níže uvedených bodù.

## Úkoly k doplnìní kódu

1. Implementujte paralelní zpracování obrázku pomocí Parallel.For bez použití unsafe kódu.
1. Implementujte paralelní zpracování obrázku pomocí Parallel.For s použitím unsafe kódu.
1. Navrhnìte a realizujte lepší uživatelské rozhraní pro aplikaci (napø. pøidání možností výbìru filtrù, indikátor prùbìhu, tlaèítko pro zastavení operace).
1. (Volitelné) Refaktorujte aplikaci pro architekturu MVVM, aby byl kód pøehlednìjší a snadno rozšiøitelný.

## Vlastní filtr

Vytvoøte vlastní efektivní paralelní filtry pro zpracování obrázku. Výsledek zobrazte v aplikaci. Pokud budou Váš filry vyžadovat další ovládací prvky v uživatelském rozhraní, doplòte je a popište jejich funkènost. 
Vyberte si jeden jednoduchý (oznaèený hvìzdièkou) a jeden složitìjší.

## Popis vybraných algoritmù

### Zvýraznìní hran
- **Jak funguje**: Detekuje hrany na základì zmìn jasu nebo barev v obraze. Nejèastìji se používá Sobelùv operátor, Cannyho detekce hran nebo Laplaceùv operátor.
- **Princip**: Aplikuje se konvoluèní filtr s maticí detekující zmìny intenzity v x a y smìru.
- **Odkaz**: [Sobel a Laplaceovy filtry na Wikipedia](https://en.wikipedia.org/wiki/Sobel_operator)

### Zvýraznìní oblastí s vysokým kontrastem
- **Jak funguje**: Vyhledává oblasti s rychlými zmìnami intenzity mezi sousedními pixely a zvýrazòuje je.
- **Princip**: Aplikace vysokofrekvenèního filtru (napø. pomocí Fourierovy transformace).
- **Odkaz**: [Fourierova analýza obrazu](https://en.wikipedia.org/wiki/Fourier_transform)

### Zvýraznìní oblastí s nízkým kontrastem 
- **Jak funguje**: Naopak, hledá oblasti s pomalými zmìnami intenzity a zvyšuje jejich viditelnost.
- **Princip**: Použití nízkofrekvenèního filtru na odstranìní vysokých detailù.
- **Odkaz**: [Gaussian Blur na Wikipedia](https://en.wikipedia.org/wiki/Gaussian_blur)

### Zvýraznìní oblastí s vysokým jasem (*)
- **Jak funguje**: Zvýrazòuje pixely, jejichž jas (luminance) pøekraèuje urèitou mez.
- **Princip**: Luminance je obvykle vypoèítána jako vážená kombinace RGB hodnot: \( Y = 0.2126R + 0.7152G + 0.0722B \).
- **Odkaz**: [Luminance a barvy](https://en.wikipedia.org/wiki/Relative_luminance)

### Zvýraznìní oblastí s nízkým jasem (*)
- **Jak funguje**: Zamìøuje se na pixely s nízkou luminancí.
- **Princip**: Podobný jako u vysokého jasu, ale se zamìøením na dolní prahovou hodnotu.
- **Odkaz**: Viz luminance výše.

### Zvýraznìní oblastí s vysokou sytostí (*)
- **Jak funguje**: Identifikuje oblasti s intenzivní barvou (saturace) a zvýrazní je.
- **Princip**: Saturace je vypoèítána v HSV nebo HSL modelu. V HSV se saturace urèuje jako \( S = \frac{max - min}{max} \).
- **Odkaz**: [HSV model na Wikipedia](https://en.wikipedia.org/wiki/HSL_and_HSV)

### Zvýraznìní oblastí s nízkou sytostí (*)
- **Jak funguje**: Zamìøuje se na oblasti s nízkou saturací, které jsou témìø šedé.
- **Princip**: Filtrace barev se saturací pod urèitou hranici.
- **Odkaz**: Viz HSV model výše.

### Pøevod na èernobílý obrázek nastavením prahu jasu (*)
- **Jak funguje**: Každý pixel se porovná s prahovou hodnotou. Pokud je vyšší, pixel je bílý, jinak èerný.
- **Princip**: \( grayscale > threshold ? white : black \).
- **Odkaz**: [Binarizace na Wikipedia](https://en.wikipedia.org/wiki/Thresholding_(image_processing))

### Pøevod na èernobílý obrázek pomocí výpoètu brightness (*)
- **Jak funguje**: Brightness (jas) je jednoduchý prùmìr RGB kanálù: \( B = \frac{R + G + B}{3} \).
- **Princip**: Tento pøístup mùže být ménì pøesný než luminance.
- **Odkaz**: Viz základní barvy výše.

### Pøevod na èernobílý obrázek pomocí výpoètu luminance (*)
- **Jak funguje**: Pøesnìjší než brightness, používá vážený prùmìr RGB hodnot (viz zvýraznìní vysokého jasu).
- **Odkaz**: [Luminance](https://en.wikipedia.org/wiki/Relative_luminance)

### Rozmazání obrázku pomocí prùmìrování pixelù v daném segmentu
- **Jak funguje**: Pro každý pixel se vypoèítá prùmìr hodnot v okolním oknì.
- **Princip**: Aplikuje se konvoluèní filtr s prùmìrovací maskou (box filter).
- **Odkaz**: [Box blur](https://en.wikipedia.org/wiki/Box_blur)

### Rozmazání obrázku pomocí mediánového filtru
- **Jak funguje**: Namísto prùmìru se v oknì vybere medián hodnot.
- **Princip**: Efektivní na odstranìní šumu (napø. sùl a pepø).
- **Odkaz**: [Median filter](https://en.wikipedia.org/wiki/Median_filter)

### Invertování barev – Pøevrácení barevných hodnot (*)
- **Jak funguje**: Každý pixel \( (R, G, B) \) je pøeveden na \( (255-R, 255-G, 255-B) \).
- **Odkaz**: [Color inversion](https://en.wikipedia.org/wiki/Color_negative)

### Zmìna sytosti – Zvýšení nebo snížení sytosti barev podle nastavení
- **Jak funguje**: Modifikuje saturaci v HSV nebo HSL barevném modelu. Zvýšení sytosti zintenzivní barvy, snížení je pøibližuje šedé.
- **Princip**: Sytost se poèítá jako \( S = \frac{max - min}{max} \). Zmìna sytosti zahrnuje pøenásobení této hodnoty koeficientem.
- **Odkaz**: [HSV model na Wikipedia](https://en.wikipedia.org/wiki/HSL_and_HSV)

### Zmìna kontrastu (*) podle nastavení
- **Jak funguje**: Zvýšení kontrastu rozšíøí rozdíly mezi tmavými a svìtlými oblastmi, snížení kontrastu tyto rozdíly zmenší.
- **Princip**: Aplikuje se transformace na pixelové hodnoty, napø. \( newValue = (oldValue - 128) \times factor + 128 \).
- **Odkaz**: [Kontrast v obrazech](https://en.wikipedia.org/wiki/Contrast_(vision))

### Zvýraznìní barevného odstínu (*)
- **Jak funguje**: Zvýrazòuje nebo snižuje intenzitu urèitého odstínu v barevném spektru.
- **Princip**: V HSV modelu se upraví pouze H (hue), ostatní hodnoty (sytost a jas) zùstávají nezmìnìné.
- **Odkaz**: Viz [HSV model](https://en.wikipedia.org/wiki/HSL_and_HSV)

### Gamma korekce (*)
- **Jak funguje**: Zmìní jas obrazu pomocí exponentiální transformace. Používá se pro kompenzaci nelinearity lidského vnímání.
- **Princip**: Transformace pixelù podle \( newValue = 255 \times \left(\frac{oldValue}{255}\right)^\gamma \).
- **Odkaz**: [Gamma korekce](https://en.wikipedia.org/wiki/Gamma_correction)

### Selektivní barevnost (*) s výbìrem odstínu
- **Jak funguje**: Zachová jednu barvu (napø. èervenou) a odbarví ostatní èásti obrazu.
- **Princip**: Používá se maska pro zachování barev v urèitém rozsahu odstínù, ostatní barvy se pøevedou na stupnì šedi.
- **Odkaz**: [Selective colorization tutorial](https://docs.gimp.org/en/)

### Posterizace (*?)
- **Jak funguje**: Snižuje poèet barev v obrazu, což vytváøí efekt s ostrými pøechody.
- **Princip**: Pixelové hodnoty se zaokrouhlují na nejbližší hodnotu v omezené paletì.
- **Odkaz**: [Posterization](https://en.wikipedia.org/wiki/Posterization)

### Pixelace
- **Jak funguje**: Obrázek se rozdìlí na vìtší bloky (pixely), které mají stejnou barvu.
- **Princip**: Pro každý blok se vypoèítá prùmìrná barva a aplikuje se na celý blok.
- **Odkaz**: [Pixelation](https://en.wikipedia.org/wiki/Pixelation)

### Sepia efekt (*)
- **Jak funguje**: Pøevede obraz do hnìdých tónù, což vytváøí starobylý vzhled.
- **Princip**: Kombinuje RGB kanály s váženými koeficienty: 
  \( R' = 0.393R + 0.769G + 0.189B \), 
  \( G' = 0.349R + 0.686G + 0.168B \), 
  \( B' = 0.272R + 0.534G + 0.131B \).
- **Odkaz**: [Sepia toning](https://en.wikipedia.org/wiki/Sepia_toning)

### Vignette efekt
- **Jak funguje**: Ztmavuje okraje obrazu a zvýrazòuje støed.
- **Princip**: Intenzita pixelù na okrajích je zmenšena podle jejich vzdálenosti od støedu obrazu.
- **Odkaz**: [Vignette efekt](https://en.wikipedia.org/wiki/Vignetting)

### Pøidání šumu (*)
- **Jak funguje**: Pøidává náhodné hodnoty k pixelùm, aby se simuloval šum (napø. šum kamery).
- **Princip**: Ke každému pixelu se pøiète náhodná hodnota z urèitého rozsahu.
- **Odkaz**: [Noise in images](https://en.wikipedia.org/wiki/Image_noise)

### Odstranìní šumu
- **Jak funguje**: Používá filtraci (napø. prùmìrování nebo gaussovské rozmazání), aby odstranil nežádoucí šum.
- **Princip**: Nahrazení hodnoty pixelu prùmìrem hodnot z jeho okolí.
- **Odkaz**: [Noise reduction](https://en.wikipedia.org/wiki/Noise_reduction)

### Embossing
- **Jak funguje**: Vytváøí reliéfní vzhled obrazu simulací 3D efektu.
- **Princip**: Aplikuje se konvoluèní filtr s maticí zvýrazòující pøechody mezi sousedními pixely.
- **Odkaz**: [Embossing filter](https://en.wikipedia.org/wiki/Image_embossing)

### Sharpening
- **Jak funguje**: Zvýrazòuje hrany v obrazu, aby vypadal ostøeji.
- **Princip**: Kombinuje pùvodní obraz s jeho zvýraznìnými hranami.
- **Odkaz**: [Image sharpening](https://en.wikipedia.org/wiki/Unsharp_masking)

### Detekce barevných oblastí (*) s výbìrem barvy
- **Jak funguje**: Zvýrazòuje oblasti s urèitou dominantní barvou (napø. zelené listy).
- **Princip**: Barevné oblasti se identifikují podle HSV modelu.
- **Odkaz**: [Color detection in HSV](https://en.wikipedia.org/wiki/HSL_and_HSV)

### Rotace barevného spektra (*)
- **Jak funguje**: Posouvá barvy v HSV modelu (napø. èervená se zmìní na modrou).
- **Princip**: Hodnota H (hue) se pøiète nebo odeète o urèitou konstantu.
- **Odkaz**: Viz HSV model výše.

### Kaleidoskopický efekt (* * *)
- **Jak funguje**: Zrcadlí èásti obrazu podle os symetrie.
- **Princip**: Pixely jsou pøemapovány podle pravidel symetrie.
- **Odkaz**: [Kaleidoscopic effect tutorial](https://docs.gimp.org/en/)

### Skládání kanálù (*)
- **Jak funguje**: Zámìna barevných kanálù mezi sebou (napø. èervený za zelený).
- **Princip**: Manipulace s hodnotami R, G a B v každém pixelu.
- **Odkaz**: [Channel swapping in images](https://en.wikipedia.org/wiki/RGB_color_model)

### Vyhlazení
- **Jak funguje**: Zmìkèuje obraz odstranìním vysokofrekvenèních detailù.
- **Princip**: Aplikuje se gaussovský nebo boxový filtr.
- **Odkaz**: [Smoothing filters](https://en.wikipedia.org/wiki/Gaussian_blur)

### Binarizace (*)
- **Jak funguje**: Pøevádí obraz na èernobílý podle nastaveného prahu.
- **Princip**: Pokud je hodnota pixelu nad prahem, je nastavena na bílou, jinak na èernou.
- **Odkaz**: Viz [Thresholding](https://en.wikipedia.org/wiki/Thresholding_(image_processing))

### Kreslení kontur
- **Jak funguje**: Zvýrazòuje linie v obrazu podle pøechodù jasu.
- **Princip**: Aplikuje se Laplaceùv nebo Sobelùv filtr.
- **Odkaz**: [Edge detection](https://en.wikipedia.org/wiki/Edge_detection)

### Zvýraznìní textury
- **Jak funguje**: Zvýrazòuje drobné detaily a hrubosti povrchu.
- **Princip**: Aplikuje se vysokofrekvenèní filtr.
- **Odkaz**: [Texture enhancement](https://en.wikipedia.org/wiki/Image_texture)

### Barevné rozdìlení (* * *)
- **Jak funguje**: Rozdìluje obraz na oblasti podle barevných segmentù.
- **Princip**: Používá se k-means clustering nebo podobná metoda.
- **Odkaz**: [Color segmentation](https://en.wikipedia.org/wiki/Image_segmentation)

### Gradientní tónování
- **Jak funguje**: Pøidává pøes obraz barevný gradient.
- **Princip**: Pøièítají se barevné hodnoty podle polohy pixelu v obraze.
- **Odkaz**: [Gradient overlay tutorial](https://docs.gimp.org/en/)

### Simulace barevné slepoty
- **Jak funguje**: Zmìní barevné spektrum tak, aby simulovalo urèitý typ barevné slepoty (napø. deuteranopie).
- **Princip**: Barevné kanály jsou transformovány podle pøíslušného modelu slepoty.
- **Odkaz**: [Color blindness simulation](https://en.wikipedia.org/wiki/Color_blindness)

### Prolínání dvou obrázkù (*?) s výbìrem míry prolínání
- **Jak funguje**: Opticky zprùmìruje hodnoty pixelù z dvou obrázkù.
- **Princip**: Pro každý pixel se vypoèítá vážený prùmìr hodnot z obou obrázkù.
- **Odkaz**: [Cross-fading tutorial](https://docs.gimp.org/en/)

### Pohybové rozmazání dle daného vektoru
- **Jak funguje**: Simuluje pohyb kamery nebo objektu v obraze.
- **Princip**: Pixelové hodnoty se prùmìrují podle smìru pohybu.
- **Odkaz**: [Motion blur](https://en.wikipedia.org/wiki/Motion_blur)

### Plátno s texturou
- **Jak funguje**: Pøidává texturu na obraz, aby vypadal jako malba na plátnì.
- **Princip**: Textura se pøekrývá s obrazem a mùže být prùhledná.
- **Odkaz**: [Canvas texture tutorial](https://docs.gimp.org/en/)

### Vlastní výpoèet algoritmu HOG (* * *)
- **Jak funguje**: Vytváøí histogram orientovaných gradientù pro detekci objektù.
- **Princip**: Pro každý blok o dané velikosti se vypoèítá gradient a jeho orientace, které se seskupují do histogramù.
- **Odkaz**: [Histogram of oriented gradients](https://en.wikipedia.org/wiki/Histogram_of_oriented_gradients)

### Jakýkoli jiný vlastní filtr
- **Inspirace**: GIMP, Photopea, Photoshop, Lightroom, ...

Poznámka: Pøi návrhu filtru dbejte na efektivní využití paralelního zpracování.