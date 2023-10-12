using System;
using System.IO;
using System.Numerics;

class Config { // Configurações da imagem
  public static int WIDTH = 100;
  public static int HEIGHT = 100;
  public static Vector3 red = new Vector3(255,0,0);
  public static Vector3 green = new Vector3(0,255,0);
  public static Vector3 blue = new Vector3(0,0,255);
  public static Vector3 white = new Vector3(255,255,255);
  public static float ambient = 0.2f;

}
class SaveImage { // Criando inicio do arquivo de texto
  public static void Save(string name, string s){
    string srt = "P3\n" + Config.WIDTH + " " + Config.HEIGHT + "\n255\n";
    srt += s;
    File.WriteAllText(name +".ppm" , srt);
  }
}

public class Camera { //Criando camera
  public Vector3 pos, dir;
  float fov, w, h ,aspect;


  public Camera(Vector3 p, Vector3 d, float fov){ // construtor da camera
    pos = p;
    dir = d;
    this.fov = fov * MathF.PI / 180.0f;
    w = Config.WIDTH;
    h= Config.HEIGHT;
    aspect = w / h;
  }

  public void Lookto(int x , int y){
    float twoTan = MathF.Tan(fov / 2.0f);
    float xNorm = 2.0f * (x/w) - 1.0f;
    float yNorm = 1.0f - 2.0f * (y/h);
    float Px = xNorm * twoTan * aspect;
    float Py = yNorm * twoTan;
    dir = Vector3.Normalize(new Vector3(Px, - Py, 1));
  }
  public Vector3 getPoint(float d){
    return pos + dir * d;
  }
}
public class Light{ //criando luz
  public Vector3 pos;
  public Light (Vector3 p){
    pos = p;
  }
}
public class Sphere{ // criando esfera
  public Vector3 center;
  public float radius;
  public float radius2;
  public Vector3 color;

  public Sphere (Vector3 c, float r, Vector3 color){ // construtor da esfera
    center = c;
    radius = r;
    radius2 = r*r;
    this.color = color;
  }

  public float Interscection(Camera r){
    Vector3 d = center - r.pos;
    float tca = Vector3.Dot(d, r.dir);
    if (tca <= 0) return -1.0f;
    float d2  = Vector3.Dot(d,d) - tca*tca;
    if (d2 > radius2) return -1.0f;
    return tca - MathF.Sqrt(radius2 -d2);
     
  }
  public Vector3 normal(Vector3 p){
    Vector3 n = p - center;
    return Vector3.Normalize(n);
  }
}

public class Buffer{
  public Vector3 [,] frame;
  
  public Buffer(int w, int h){
   Config.WIDTH = w;
   Config.HEIGHT = h;
    frame = new Vector3 [Config.WIDTH, Config.HEIGHT];
 }
  public void Clear(Vector3 color){
    for (int h = 0; h < Config.HEIGHT; h++){
      for (int w = 0; w < Config.WIDTH; w++){
          frame[w, h] = color;
      }  
    }
  }
 public void SetPixel(int x, int y, Vector3 color){
   x = Clamp (x , 0, Config.WIDTH -1);
   y = Clamp (y , 0, Config.HEIGHT -1);
   color = Vector3.Clamp(color, Vector3.Zero, Vector3.One*255);
   frame[x ,y] = color;
 }

 public void Raystrace (Light light, Camera cam, Sphere s ){
   for (int h = 0; h < Config.HEIGHT; h++){
      for (int w = 0; w < Config.WIDTH; w++){
        cam.Lookto(w,h);
        float hit = s.Interscection(cam);
        if(hit > 0){
          Vector3 lightdir = light.pos - s.center;
          lightdir = Vector3.Normalize(lightdir);
          Vector3 normal = s.normal(cam.getPoint(hit));
          Vector3 finalColor = BlinnPhong(normal, -cam.dir, lightdir, s.color);
          SetPixel (w,h, finalColor);
        }
      }
   }
 }
 Vector3 BlinnPhong(Vector3 normal, Vector3 viewDir, Vector3 lightdir, Vector3 color){ // adicionando luz 
   Vector3 finalColor = new Vector3 (0,0,0);
   float diff = Vector3.Dot(normal,lightdir);
   Vector3 h = Vector3.Normalize(lightdir + viewDir);
   float nh = Vector3.Dot(normal,h);
   float spec = MathF.Pow(nh, 50.0f);
   return color * Config.ambient + color * diff + Config.white * spec;
 }

 int Clamp(int v, int min, int max){
   return (v < min)? min: (v > max)? max:v;
 }
 public override string ToString(){ // convertendo informações para texto
   string s ="";
   for (int h = 0; h < Config.HEIGHT; h++){
      for (int w = 0; w < Config.WIDTH; w++){
        s += (int)frame[w,h].X + " " + (int)frame[w, h].Y + " " + (int)frame[w,h].Z + " ";
      }
      s+= "\n";
    }
    return s;
 }
 public void Save(string name){ // salvando imagem
   SaveImage.Save(name, ToString());
 }
} 

class Program {
  public static void Main (string[] args) {
    //Criando uma nova image
    Buffer img = new Buffer (150,150);
    img.Clear(new Vector3 (100,60,100));
    Light light = new Light(new Vector3(7,-5,0));
    // Adicionando objetos
    Camera camera = new Camera(new Vector3 (0,0,0), new Vector3(0,0,1), 60.0f );using System;
using System.IO;
using System.Numerics;

// Classe que contém configurações da imagem
class Config {
  public static int WIDTH = 100; // Largura da imagem
  public static int HEIGHT = 100; // Altura da imagem
  public static Vector3 red = new Vector3(255, 0, 0); // Cor vermelha
  public static Vector3 green = new Vector3(0, 255, 0); // Cor verde
  public static Vector3 blue = new Vector3(0, 0, 255); // Cor azul
  public static Vector3 white = new Vector3(255, 255, 255); // Cor branca
  public static float ambient = 0.2f; // Intensidade da luz ambiente
}

// Classe para salvar imagens em formato PPM
class SaveImage {
  public static void Save(string name, string s) {
    // Monta o cabeçalho do arquivo PPM com base nas configurações de Config
    string srt = "P3\n" + Config.WIDTH + " " + Config.HEIGHT + "\n255\n";
    srt += s;
    // Escreve o conteúdo da imagem no arquivo PPM
    File.WriteAllText(name + ".ppm", srt);
  }
}

// Classe para representar uma câmera
public class Camera {
  public Vector3 pos, dir;
  float fov, w, h, aspect;

  public Camera(Vector3 p, Vector3 d, float fov) {
    // Construtor da câmera
    pos = p;
    dir = d;
    this.fov = fov * MathF.PI / 180.0f;
    w = Config.WIDTH;
    h = Config.HEIGHT;
    aspect = w / h;
  }

  public void Lookto(int x, int y) {
    // Define a direção da câmera com base nas coordenadas de tela
    float twoTan = MathF.Tan(fov / 2.0f);
    float xNorm = 2.0f * (x / w) - 1.0f;
    float yNorm = 1.0f - 2.0f * (y / h);
    float Px = xNorm * twoTan * aspect;
    float Py = yNorm * twoTan;
    dir = Vector3.Normalize(new Vector3(Px, -Py, 1));
  }

  public Vector3 getPoint(float d) {
    // Calcula um ponto ao longo da direção da câmera
    return pos + dir * d;
  }
}

// Classe para representar uma fonte de luz
public class Light {
  public Vector3 pos;

  public Light(Vector3 p) {
    pos = p;
  }
}

// Classe para representar uma esfera
public class Sphere {
  public Vector3 center;
  public float radius;
  public float radius2;
  public Vector3 color;

  public Sphere(Vector3 c, float r, Vector3 color) {
    // Construtor da esfera
    center = c;
    radius = r;
    radius2 = r * r;
    this.color = color;
  }

  public float Interscection(Camera r) {
    // Verifica a interseção de um raio (linha de visão) com a esfera
    Vector3 d = center - r.pos;
    float tca = Vector3.Dot(d, r.dir);
    if (tca <= 0) return -1.0f;
    float d2 = Vector3.Dot(d, d) - tca * tca;
    if (d2 > radius2) return -1.0f;
    return tca - MathF.Sqrt(radius2 - d2);
  }

  public Vector3 normal(Vector3 p) {
    // Calcula a normal em um ponto da superfície da esfera
    Vector3 n = p - center;
    return Vector3.Normalize(n);
  }
}

// Classe para gerenciar um buffer de pixels e renderizar imagens
public class Buffer {
  public Vector3[,] frame;

  public Buffer(int w, int h) {
    // Construtor do buffer
    Config.WIDTH = w;
    Config.HEIGHT = h;
    frame = new Vector3[Config.WIDTH, Config.HEIGHT];
  }

  public void Clear(Vector3 color) {
    // Preenche o buffer com uma cor específica
    for (int h = 0; h < Config.HEIGHT; h++) {
      for (int w = 0; w < Config.WIDTH; w++) {
        frame[w, h] = color;
      }
    }
  }

  public void SetPixel(int x, int y, Vector3 color) {
    // Define a cor de um pixel no buffer
    x = Clamp(x, 0, Config.WIDTH - 1);
    y = Clamp(y, 0, Config.HEIGHT - 1);
    color = Vector3.Clamp(color, Vector3.Zero, Vector3.One * 255);
    frame[x, y] = color;
  }

  public void Raystrace(Light light, Camera cam, Sphere s) {
    // Renderiza uma cena com uma esfera e uma fonte de luz
    for (int h = 0; h < Config.HEIGHT; h++) {
      for (int w = 0; w < Config.WIDTH; w++) {
        cam.Lookto(w, h);
        float hit = s.Interscection(cam);
        if (hit > 0) {
          Vector3 lightdir = light.pos - s.center;
          lightdir = Vector3.Normalize(lightdir);
          Vector3 normal = s.normal(cam.getPoint(hit));
          Vector3 finalColor = BlinnPhong(normal, -cam.dir, lightdir, s.color);
          SetPixel(w, h, finalColor);
        }
      }
    }
  }

  Vector3 BlinnPhong(Vector3 normal, Vector3 viewDir, Vector3 lightdir, Vector3 color) {
    // Modelo de reflexão Blinn-Phong para calcular a cor final
    Vector3 finalColor = new Vector3(0, 0, 0);
    float diff = Vector3.Dot(normal, lightdir);
    Vector3 h = Vector3.Normalize(lightdir + viewDir);
    float nh = Vector3.Dot(normal, h);
    float spec = MathF.Pow(nh, 50.0f);
    return color * Config.ambient + color * diff + Config.white * spec;
  }

  int Clamp(int v, int min, int max) {
    // Garante que um valor está dentro de um intervalo específico
    return (v < min) ? min : (v > max) ? max : v;
  }

  public override string ToString() {
    // Converte o conteúdo do buffer em uma representação de string
    string s = "";
    for (int h = 0; h < Config.HEIGHT; h++) {
      for (int w = 0; w < Config.WIDTH; w++) {
       
        // Converte as componentes do pixel para inteiros e as concatena na string
        s += (int)frame[w, h].X + " " + (int)frame[w, h].Y + " " + (int)frame[w, h].Z + " ";
      }
      s += "\n"; // Adiciona uma quebra de linha após cada linha de pixels
    }
    return s; // Retorna a representação de string do buffer
  }

  public void Save(string name) {
    // Salva o conteúdo do buffer em um arquivo PPM
    SaveImage.Save(name, ToString());
  }
}

class Program {
  public static void Main(string[] args) {
    // Criando um novo buffer de imagem
    Buffer img = new Buffer(150, 150);
    img.Clear(new Vector3(100, 60, 100)); // Define a cor de fundo

    Light light = new Light(new Vector3(7, -5, 0)); // Cria uma fonte de luz

    // Adicionando objetos à cena
    Camera camera = new Camera(new Vector3(0, 0, 0), new Vector3(0, 0, 1), 60.0f);
    Sphere sphere1 = new Sphere(new Vector3(1, 0, 10), 3.0f, Config.red);
    Sphere sphere2 = new Sphere(new Vector3(-1.5f, 0.5f, 6), 1.5f, Config.blue);
    Sphere sphere3 = new Sphere(new Vector3(2, 1, 6), 1.0f, Config.green);

    // Renderizando objetos na cena
    img.Raystrace(light, camera, sphere1);
    img.Raystrace(light, camera, sphere2);
    img.Raystrace(light, camera, sphere3);

    img.Save("Imagem"); // Salvando a imagem resultante
  }
}

