using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// LUMI: EL VALLE DETENIDO
// "La oscuridad no mata. Congela el tiempo."
// Controla todo el juego: titulo, mini eventos de la historia, construir
// cada nivel desde un mapa de texto (instanciando prefabs), muertes,
// la pelea contra La Bruma y la revelacion final.
public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    [Header("Personajes")]
    public GameObject prefabLumi;
    public GameObject prefabNadieJugable;
    public GameObject prefabNadieCompanero;
    public GameObject prefabBruma;
    public GameObject prefabAbuelaVela;
    public GameObject prefabAldeano;
    public GameObject prefabSombra;

    [Header("Escenario")]
    public GameObject prefabBloque;
    public GameObject prefabFondo;
    public GameObject prefabPincho;
    public GameObject prefabEspinaBruma;
    public GameObject prefabGota;
    public GameObject prefabEscombro;
    public GameObject prefabRoca;
    public GameObject prefabEstalactita;
    public GameObject prefabCascada;
    public GameObject prefabSeta;
    public GameObject prefabAscensor;
    public GameObject prefabReloj;
    public GameObject prefabCristal;
    public GameObject prefabFarol;
    public GameObject prefabPilar;
    public GameObject prefabFaro;
    public GameObject prefabFaroFalso;
    public GameObject prefabChispa;
    public GameObject prefabGlitch;
    public GameObject prefabLuciernaga;
    public GameObject prefabPedazo;
    public GameObject prefabPolvo;

    [Header("UI")]
    public GameObject hud;
    public GameObject panelTitulo;
    public GameObject panelEvento;
    public GameObject panelError;
    public GameObject panelFalla;
    public GameObject panelFinal;
    public Text textoNivel;
    public Text textoPista;
    public Text textoMuertes;
    public Text textoAviso;
    public Text textoDialogo;
    public Text textoEmpezar;
    public Text textoEventoTitulo;
    public Text textoEventoCuerpo;
    public Text textoError;
    public Text textoFinal;
    public Image[] cuadros;

    [Header("Musica (original, sin copyright)")]
    public AudioClip musicaValle;
    public AudioClip musicaBruma;
    private AudioSource musica;

    [Header("Efectos de sonido (originales)")]
    public AudioSource fuenteEfectos;
    public AudioClip[] efectos;
    private Dictionary<string, float> ultimoSonido = new Dictionary<string, float>();

    [Header("Colores")]
    public Color colorLumi = new Color(1f, 0.54f, 0.12f);
    public Color[] coloresMundo;   // fondo de la sala: bosque, cuevas, ciudad, gran faro

    // ===== ESTADO =====
    enum Estado { Titulo, Evento, Jugando, Error, Final }
    private Estado estado = Estado.Titulo;
    private int nivelActual = 0;
    private int muertes = 0;
    private bool cambiandoNivel = false;
    private Transform contenedor;
    private Lumi lumi;
    private Bruma bruma;
    [HideInInspector] public int pilaresEncendidos = 0;
    [HideInInspector] public bool luzApagada = false;

    // Eventos (paginas de texto)
    private string[][] paginas;
    private int paginaActual;
    private Action alTerminarEvento;
    private float tiempoError;
    private float tiempoFalla = -1f;

    // ===== MAPAS =====
    // #  pared          .  aire           P  Lumi           N  Nadie (jugable)
    // D  faro           F  faro falso     S  chispa de Lumi (meta de Nadie)
    // g  gota congelada (cae si la iluminas)     x  escombro (cae si lo iluminas)
    // O  roca (cae y aplasta)                    C  estalactita (cae si la iluminas)
    // ^  espinas        H  espinas de La Bruma (salen en la oscuridad)
    // E  sombra (estatua en la oscuridad, despierta con luz)
    // R  seta saltarina (solo con luz)   A  ascensor   < >  reloj que se mueve con luz
    // W  cascada congelada (solida sin luz, se atraviesa con luz)
    // K  aldeano congelado (solido sin luz)   V  Abuela Vela   Z  cristal
    // l  farol           I  pilar del Gran Faro   !  el juego se apaga
    private string[][] mapas = new string[][]
    {
        new string[] {  // Nivel 1
            "################################",
            "################################",
            "################################",
            "################################",
            "################################",
            "###..........................###",
            "###..........................###",
            "###..........................###",
            "###..........................###",
            "###...........g.g............###",
            "###.........g.....g..........###",
            "###.P.....g.........g.g...D..###",
            "#########..............#########",
            "#########..............#########",
            "#########..............#########",
            "#########..............#########",
            "#########..............#########",
            "#########..............#########"
        },
        new string[] {  // Nivel 2
            "################################",
            "################################",
            "################################",
            "################################",
            "################################",
            "###..........................###",
            "###..........................###",
            "###..........................###",
            "###..........................###",
            "###......C.C.................###",
            "###..........................###",
            "###.P.V................E..D..###",
            "##############xxxxxx############",
            "##############......############",
            "##############......############",
            "##############......############",
            "##############......############",
            "##############......############"
        },
        new string[] {  // Nivel 3
            "################################",
            "################################",
            "################################",
            "################################",
            "################################",
            "###..........................###",
            "###..........................###",
            "###.....................E..D.###",
            "###....................#########",
            "###............g.g...........###",
            "###..........g.....g.........###",
            "###.P.HHH.............R.HHHHH###",
            "###########..........###########",
            "###########..........###########",
            "###########..........###########",
            "###########..........###########",
            "###########..........###########",
            "###########..........###########"
        },
        new string[] {  // Nivel 4
            "################################",
            "################################",
            "################################",
            "################################",
            "################################",
            "###..........l.......l.......###",
            "###..........................###",
            "###..........................###",
            "###..........................###",
            "###..........................###",
            "###.....g.g.g.g.g...g.g......###",
            "###.N..HHHHHHHHHHHEHHHHHH.S..###",
            "################################",
            "################################",
            "################################",
            "################################",
            "################################",
            "################################"
        },
        new string[] {  // Nivel 5
            "################################",
            "################################",
            "################################",
            "################################",
            "################################",
            "###..........................###",
            "###..........................###",
            "###..........................###",
            "###..........................###",
            "###........................C.###",
            "###.....Z....................###",
            "###.P.V................<...D.###",
            "##########................######",
            "##########................######",
            "##########................######",
            "##########................######",
            "##########................######",
            "##########................######"
        },
        new string[] {  // Nivel 6
            "################################",
            "################################",
            "################################",
            "################################",
            "################################",
            "###......W...................###",
            "###......W...................###",
            "###......W.................D.###",
            "###......W...............#######",
            "###....C.W.......g...........###",
            "###......W...g.g...g.........###",
            "###.P....W............A...E..###",
            "###########..........###########",
            "###########..........###########",
            "###########..........###########",
            "###########..........###########",
            "###########..........###########",
            "###########..........###########"
        },
        new string[] {  // Nivel 7
            "################################",
            "################################",
            "################################",
            "################################",
            "################################",
            "###..........................###",
            "###........................D.###",
            "###..........C...C......########",
            "###..........................###",
            "###....................K.....###",
            "###....>.....................###",
            "###.PV....................F..###",
            "#######..................#######",
            "#######..................#######",
            "#######..................#######",
            "#######..................#######",
            "#######..................#######",
            "#######..................#######"
        },
        new string[] {  // Nivel 8
            "################################",
            "################################",
            "################################",
            "################################",
            "################################",
            "###!.........................###",
            "###..........................###",
            "###.............#............###",
            "###.............#............###",
            "###.............#...C........###",
            "###.............#............###",
            "###.P...HHHHHA..#.HHHHH..E.D.###",
            "################################",
            "################################",
            "################################",
            "################################",
            "################################",
            "################################"
        },
        new string[] {  // Nivel 9
            "################################",
            "################################",
            "################################",
            "################################",
            "################################",
            "###..........................###",
            "###..........................###",
            "###..................C.......###",
            "###..........................###",
            "###.......g...Z...........V.D###",
            "###.....g...g.............######",
            "###.P...................<....###",
            "#######......###..........######",
            "#######......###..........######",
            "#######......###..........######",
            "#######......###..........######",
            "#######......###..........######",
            "#######......###..........######"
        },
        new string[] {  // Nivel 10
            "################################",
            "################################",
            "################################",
            "################################",
            "################################",
            "###..........................###",
            "###..I.......................###",
            "########...g....I............###",
            "###...........#####..........###",
            "###..........................###",
            "###..........................###",
            "###.P......A........HHH..I...###",
            "################################",
            "################################",
            "################################",
            "################################",
            "################################",
            "################################"
        },
    };

    private string[] nombres =
    {
        "LA LLUVIA QUE NO CAE",
        "SI ILUMINAS LO QUE PISAS...",
        "EL APAGÓN",
        "NADIE",
        "CUEVAS DE CRISTAL",
        "LO QUE NADIE ESCONDÍA",
        "LA CIUDAD DETENIDA",
        "EL JUEGO SE APAGA",
        "EL CORO DEL VALLE",
        "EL GRAN FARO"
    };

    private string[] pistas =
    {
        "Mantén Q para encoger tu luz. Lo que no iluminas no se mueve.",
        "Lo azul está congelado: es suelo... mientras no lo ilumines.",
        "Las espinas de La Bruma salen en la oscuridad.",
        "Eres Nadie. En la oscuridad todo es suelo, pero la luz te quema.",
        "Ilumina el cristal: su luz llega donde tú no llegas.",
        "Tu luz derrite cascadas. No las congeles contigo adentro.",
        "Los relojes avanzan con tu luz. SHIFT: destello.",
        "ERR0R  ·  99:59:59  ·  ERR0R",
        "Todo lo que aprendiste. El Gran Faro te espera.",
        "Enciende los 3 pilares. Ella se mueve como tú."
    };

    private Dictionary<int, string> frasesAbuela = new Dictionary<int, string>
    {
        { 1, "«...otra vez...»" },
        { 4, "«...tú...»" },
        { 6, "«...no la dejes...»" },
        { 8, "«...hacerlo otra vez.»" }
    };

    // Mini eventos que aparecen ANTES de cada nivel
    private Dictionary<int, string[]> eventos = new Dictionary<int, string[]>
    {
        { 0, new[] {
            "LA OSCURIDAD NO MATA|Congela el tiempo.\n\nNueve faros mantenían el tiempo del valle en marcha.\nUna noche, La Bruma se los tragó y todo quedó congelado.\nUn niño quedó a mitad de un salto. Un panadero, con el pan en el aire.",
            "LUMI|De la última llama del Gran Faro nace Lumi.\nLo que ilumina se mueve. Lo que deja en sombra se queda quieto... y se puede pisar.\n\nAl primer paso conoce a Nadie, una sombrita de un ojo.\n\nMUNDO 1  ·  BOSQUE DE LUCIÉRNAGAS" } },
        { 3, new[] {
            "EL APAGÓN|La Bruma le roba la luz a Lumi.\nLumi se congela.\n\nPor primera vez juegas como Nadie:\nen la oscuridad total, todo es plataforma.\nLo que era peligro ahora es tu terreno.\n\nRecupera la chispa de Lumi." } },
        { 4, new[] {
            "EL SUEÑO|Lumi vuelve a brillar.\nEsa noche sueña con un valle bajo un sol que nunca se pone.\nNadie podía dormir. Nadie podía soñar.\n\nDespierta con un mal presentimiento.\n\nMUNDO 2  ·  CUEVAS DE CRISTAL" } },
        { 5, new[] {
            "LO QUE NADIE ESCONDÍA|En el reflejo de un cristal, Nadie no tiene sombra.\nNadie ES una sombra.\n\nNadie es parte de La Bruma.\n\nAun así, sigue a tu lado." } },
        { 6, new[] {
            "MUNDO 3  ·  CIUDAD DETENIDA|Relojes parados. Aldeanos congelados a mitad de su día.\n\nEn un muro alguien escribió:\n«LUMI ESTUVO AQUÍ»\n\nLa fecha es de hace muchos años." } },
        { 9, new[] {
            "EL CORO DEL VALLE|Ocho faros encendidos.\nLos aldeanos despiertan y cantan.\n\nEn lo alto del Gran Faro te espera La Bruma.\nDicen que pelea como tú." } },
    };

    private string[] revelacion =
    {
        "PARTIDA ANTIGUA  ·  99:59:59|Un valle bajo un sol que nunca se pone.\nUna Lumi ya ganó este juego: destruyó la noche.\nNadie podía dormir ni soñar.\nLos aldeanos le suplicaban oscuridad.",
        "LUMI = LA BRUMA|Esa Lumi se tragó su propia luz...\ny se convirtió en La Bruma.\n\nNo congeló el valle por maldad.\nLo congeló para detener el sufrimiento que ella misma causó.",
        "TÚ ERAS EL VILLANO|Faro por faro, ibas deshaciendo lo único bueno que ella hizo.\n\nNadie tiene un solo ojo\nporque es lo que queda de su cara."
    };

    void Awake()
    {
        instancia = this;
        musica = GetComponent<AudioSource>();
        Physics2D.queriesHitTriggers = false;   // los raycasts solo ven cosas solidas
    }

    void Start()
    {
        panelTitulo.SetActive(true);
        panelEvento.SetActive(false);
        panelError.SetActive(false);
        panelFalla.SetActive(false);
        panelFinal.SetActive(false);
        hud.SetActive(false);
    }

    void Update()
    {
        switch (estado)
        {
            case Estado.Titulo:
                textoEmpezar.enabled = (Time.time % 1f) < 0.7f;
                if (Entrada.Continuar()) EmpezarJuego();
                break;

            case Estado.Evento:
                if (Entrada.Continuar()) SiguientePagina();
                break;

            case Estado.Jugando:
                if (Entrada.Reiniciar() && !cambiandoNivel) CargarNivel(nivelActual);
                ActualizarFalla();
                break;

            case Estado.Error:
                tiempoError -= Time.deltaTime;
                textoError.enabled = UnityEngine.Random.value > 0.25f;
                textoError.rectTransform.anchoredPosition = new Vector2(UnityEngine.Random.Range(-8f, 8f), UnityEngine.Random.Range(-5f, 5f));
                if (tiempoError <= 0f)
                {
                    panelError.SetActive(false);
                    PonerMusica(musicaValle, 0.8f);   // el mismo tema, mas lento y triste
                    MostrarEvento(revelacion, MostrarFinal);
                }
                break;

            case Estado.Final:
                if (Entrada.Continuar()) EmpezarJuego();
                break;
        }
    }

    // ===== FLUJO =====
    // Cambia la cancion solo si es distinta (asi no se reinicia al morir)
    void PonerMusica(AudioClip clip, float tono)
    {
        if (musica == null || clip == null) return;
        musica.pitch = tono;
        if (musica.clip == clip && musica.isPlaying) return;
        musica.clip = clip;
        musica.loop = true;
        musica.Play();
    }

    // Toca un efecto por su nombre (salto, faro, muerte...)
    public void Sonar(string nombre)
    {
        if (fuenteEfectos == null || efectos == null) return;
        // si suena muchas veces seguidas (ej. varias gotas), no saturar
        if (ultimoSonido.ContainsKey(nombre) && Time.unscaledTime - ultimoSonido[nombre] < 0.06f) return;
        foreach (AudioClip c in efectos)
        {
            if (c != null && c.name == nombre)
            {
                fuenteEfectos.PlayOneShot(c);
                ultimoSonido[nombre] = Time.unscaledTime;
                return;
            }
        }
    }

    void EmpezarJuego()
    {
        Sonar("blip");
        muertes = 0;
        PonerMusica(musicaValle, 1f);
        panelTitulo.SetActive(false);
        panelFinal.SetActive(false);
        IrANivel(0);
    }

    void IrANivel(int n)
    {
        if (eventos.ContainsKey(n)) MostrarEvento(eventos[n], () => CargarNivel(n));
        else CargarNivel(n);
    }

    void SiguienteNivel()
    {
        if (nivelActual + 1 < mapas.Length) IrANivel(nivelActual + 1);
        else IniciarRevelacion();
    }

    void MostrarEvento(string[] pags, Action alTerminar)
    {
        estado = Estado.Evento;
        hud.SetActive(false);
        if (contenedor != null) Destroy(contenedor.gameObject);
        paginas = new string[pags.Length][];
        for (int i = 0; i < pags.Length; i++) paginas[i] = pags[i].Split('|');
        paginaActual = 0;
        alTerminarEvento = alTerminar;
        panelEvento.SetActive(true);
        MostrarPagina();
    }

    void MostrarPagina()
    {
        textoEventoTitulo.text = paginas[paginaActual][0];
        textoEventoCuerpo.text = paginas[paginaActual][1];
    }

    void SiguientePagina()
    {
        Sonar("blip");
        paginaActual++;
        if (paginaActual < paginas.Length) { MostrarPagina(); return; }
        panelEvento.SetActive(false);
        alTerminarEvento?.Invoke();
    }

    // ===== CONSTRUIR EL NIVEL =====
    void CargarNivel(int numero)
    {
        CancelInvoke();
        estado = Estado.Jugando;
        cambiandoNivel = false;
        nivelActual = numero;
        pilaresEncendidos = 0;
        luzApagada = false;
        tiempoFalla = -1f;
        panelFalla.SetActive(false);
        hud.SetActive(true);
        textoAviso.gameObject.SetActive(false);
        MostrarDialogo("");

        if (contenedor != null) Destroy(contenedor.gameObject);
        contenedor = new GameObject("Nivel " + (numero + 1)).transform;
        lumi = null;
        bruma = null;

        PonerMusica(numero == mapas.Length - 1 ? musicaBruma : musicaValle, 1f);

        string[] mapa = mapas[numero];
        Color colorSala = coloresMundo[Mundo(numero)];
        List<PlataformaMovil> plataformas = new List<PlataformaMovil>();
        List<Cristal> cristales = new List<Cristal>();
        Vector3 inicioLumi = Vector3.zero;

        for (int fila = 0; fila < mapa.Length; fila++)
        {
            string linea = mapa[fila];

            // Fondo de la sala (filas 5 a 11)
            int c0 = 0;
            while (fila >= 5 && fila <= 11 && c0 < linea.Length)
            {
                if (linea[c0] == '#') { c0++; continue; }
                int inicioFondo = c0;
                while (c0 < linea.Length && linea[c0] != '#') c0++;
                GameObject f = CrearBloque(prefabFondo, fila, inicioFondo, c0 - inicioFondo);
                f.GetComponent<SpriteRenderer>().color = colorSala;
            }

            int col = 0;
            while (col < linea.Length)
            {
                char c = linea[col];

                if (c == '#')
                {
                    int inicio = col;
                    while (col < linea.Length && linea[col] == '#') col++;
                    CrearBloque(prefabBloque, fila, inicio, col - inicio);
                    continue;
                }

                Vector3 pos = Celda(fila, col);
                Vector3 piso = pos + Vector3.down * 0.5f;
                Vector3 doble = pos + new Vector3(0.5f, 0f, 0f);   // objetos de 2 casillas

                switch (c)
                {
                    case 'P':
                        inicioLumi = pos + Vector3.down * 0.05f;
                        lumi = Crear(prefabLumi, inicioLumi).GetComponent<Lumi>();
                        Crear(prefabNadieCompanero, inicioLumi + new Vector3(-1.3f, 0.9f, 0f));
                        break;
                    case 'N':
                        lumi = Crear(prefabNadieJugable, pos + Vector3.down * 0.05f).GetComponent<Lumi>();
                        break;
                    case 'D': Crear(prefabFaro, piso); break;
                    case 'F': Crear(prefabFaroFalso, piso); break;
                    case 'S': Crear(prefabChispa, piso); break;
                    case 'g': Crear(prefabGota, pos); break;
                    case 'x': Crear(prefabEscombro, pos); break;
                    case 'O': Crear(prefabRoca, pos); break;
                    case 'C': Crear(prefabEstalactita, pos); break;
                    case '^': Crear(prefabPincho, pos); break;
                    case 'H': Crear(prefabEspinaBruma, pos); break;
                    case 'E': Crear(prefabSombra, pos + Vector3.down * 0.05f); break;
                    case 'R': Crear(prefabSeta, pos); break;
                    case 'W': Crear(prefabCascada, pos); break;
                    case 'K': Crear(prefabAldeano, pos); break;
                    case 'l': Crear(prefabFarol, pos); break;
                    case 'I': Crear(prefabPilar, piso); break;
                    case '!': Crear(prefabGlitch, pos); break;
                    case 'V':
                        AbuelaVela v = Crear(prefabAbuelaVela, pos).GetComponent<AbuelaVela>();
                        if (frasesAbuela.ContainsKey(numero)) v.frase = frasesAbuela[numero];
                        break;
                    case 'Z':
                        cristales.Add(Crear(prefabCristal, pos).GetComponent<Cristal>());
                        break;
                    case 'A':
                        PlataformaMovil a = Crear(prefabAscensor, doble).GetComponent<PlataformaMovil>();
                        a.recorrido = new Vector2(0f, 3f);
                        plataformas.Add(a);
                        break;
                    case '<':
                    case '>':
                        float dir = c == '>' ? 1f : -1f;
                        PlataformaMovil r = Crear(prefabReloj, doble).GetComponent<PlataformaMovil>();
                        r.recorrido = new Vector2(dir * MedirRecorrido(mapa, fila, col, (int)dir), 0f);
                        plataformas.Add(r);
                        break;
                }
                col++;
            }
        }

        // Cada cristal le manda luz a una plataforma (en orden)
        for (int i = 0; i < cristales.Count && i < plataformas.Count; i++) cristales[i].objetivo = plataformas[i];

        // Luciernagas en el bosque
        if (Mundo(numero) == 0)
        {
            for (int i = 0; i < 12; i++)
            {
                int f = UnityEngine.Random.Range(5, 11);
                int c = UnityEngine.Random.Range(4, 28);
                if (mapa[f][c] == '.') Crear(prefabLuciernaga, Celda(f, c));
            }
        }

        // En el Gran Faro aparece La Bruma donde empezo Lumi
        if (numero == mapas.Length - 1 && lumi != null)
        {
            bruma = Crear(prefabBruma, inicioLumi).GetComponent<Bruma>();
        }

        ActualizarUI();
    }

    // Cuantas casillas puede avanzar un reloj sobre el abismo
    int MedirRecorrido(string[] mapa, int fila, int col, int dir)
    {
        int pasos = 0;
        int borde = dir > 0 ? col + 2 : col - 1;
        while (true)
        {
            int c = borde + dir * pasos;
            if (c < 0 || c >= 32) break;
            if (mapa[12][c] != '.' || mapa[fila][c] != '.' || mapa[fila - 1][c] != '.') break;
            pasos++;
        }
        return pasos;
    }

    int Mundo(int nivel)
    {
        if (nivel <= 3) return 0;   // Bosque de Luciernagas
        if (nivel <= 5) return 1;   // Cuevas de Cristal
        if (nivel <= 8) return 2;   // Ciudad Detenida
        return 3;                   // El Gran Faro
    }

    Vector3 Celda(int fila, int col)
    {
        return new Vector3(col - 15.5f, 8.5f - fila, 0f);
    }

    public GameObject Crear(GameObject prefab, Vector3 pos)
    {
        return Instantiate(prefab, pos, Quaternion.identity, contenedor);
    }

    GameObject CrearBloque(GameObject prefab, int fila, int inicio, int largo)
    {
        Vector3 pos = Celda(fila, inicio) + new Vector3((largo - 1) / 2f, 0f, 0f);
        GameObject b = Crear(prefab, pos);
        b.transform.localScale = new Vector3(largo, 1f, 1f);
        return b;
    }

    public Transform LumiActual()
    {
        if (lumi == null) return null;
        return lumi.transform;
    }

    public bool DestelloActivo()
    {
        return lumi != null && lumi.DestelloActivo();
    }

    // ===== EVENTOS DEL JUEGO =====
    public void LumiMurio(Vector3 pos, bool eraNadie)
    {
        muertes++;
        Sonar("muerte");
        ActualizarUI();
        CrearPedazos(pos, eraNadie ? new Color(0.12f, 0.13f, 0.25f) : colorLumi, 10);
        CrearPedazos(pos, new Color(0.66f, 0.84f, 1f), 6);
        cambiandoNivel = true;
        Invoke(nameof(ReiniciarNivel), 0.8f);
    }

    void ReiniciarNivel()
    {
        CargarNivel(nivelActual);
    }

    public void LlegarAlFaro()
    {
        if (cambiandoNivel) return;
        cambiandoNivel = true;
        Sonar("faro");
        MostrarAviso(nivelActual == 3 ? "¡RECUPERASTE LA CHISPA!" : "¡FARO ENCENDIDO!");
        Invoke(nameof(SiguienteNivel), 1.2f);
    }

    public void PilarEncendido()
    {
        pilaresEncendidos++;
        if (pilaresEncendidos >= 3)
        {
            if (bruma != null) bruma.Desaparecer();
            MostrarAviso("¡EL GRAN FARO DESPIERTA!");
            Crear(prefabFaro, new Vector3(0.5f, -3f, 0f));
            CrearPedazos(new Vector3(0.5f, -2f, 0f), new Color(1f, 0.84f, 0.3f), 15);
        }
        else
        {
            MostrarAviso("PILAR " + pilaresEncendidos + " / 3");
            Invoke(nameof(OcultarAviso), 1.2f);
        }
    }

    void OcultarAviso()
    {
        textoAviso.gameObject.SetActive(false);
    }

    public void MostrarAviso(string texto)
    {
        textoAviso.text = texto;
        textoAviso.gameObject.SetActive(true);
    }

    public void MostrarDialogo(string texto)
    {
        textoDialogo.text = texto;
    }

    // ===== EL JUEGO SE APAGA (nivel 8) =====
    public void Falla()
    {
        if (cambiandoNivel) return;
        tiempoFalla = 2f;
        Sonar("falla");
    }

    void ActualizarFalla()
    {
        if (tiempoFalla < 0f) return;
        tiempoFalla -= Time.deltaTime;

        // 2.0 - 1.4: la pantalla parpadea (aviso)
        // 1.4 - 0.6: la luz de Lumi se apaga
        // hasta 0:   el HUD desaparece
        panelFalla.SetActive(tiempoFalla > 1.2f && (Time.time * 12f) % 2f < 1f);
        luzApagada = tiempoFalla <= 1.2f && tiempoFalla > 0.6f;
        textoNivel.enabled = textoPista.enabled = textoMuertes.enabled = tiempoFalla <= 0f;
        for (int i = 0; i < cuadros.Length; i++) cuadros[i].enabled = tiempoFalla <= 0f;

        if (tiempoFalla <= 0f)
        {
            tiempoFalla = -1f;
            panelFalla.SetActive(false);
            luzApagada = false;
        }
    }

    // ===== EL FINAL =====
    void IniciarRevelacion()
    {
        estado = Estado.Error;
        hud.SetActive(false);
        if (contenedor != null) Destroy(contenedor.gameObject);
        panelError.SetActive(true);
        tiempoError = 3f;
        if (musica != null) musica.Stop();   // silencio total en el ERROR
    }

    void MostrarFinal()
    {
        estado = Estado.Final;
        hud.SetActive(false);
        textoFinal.text = "LUMI · DÍA ETERNO · 99:59:59\n\n" +
                          "Muertes: " + muertes + "\n\n\n" +
                          "> CONTINUAR LA PARTIDA DE ELLA";
        panelFinal.SetActive(true);
    }

    // ===== EFECTOS (tambien son instanciacion) =====
    public void CrearPedazos(Vector3 pos, Color color, int cantidad)
    {
        if (contenedor == null) return;
        for (int i = 0; i < cantidad; i++)
        {
            Vector3 p = pos + (Vector3)(UnityEngine.Random.insideUnitCircle * 0.3f);
            GameObject pedazo = Crear(prefabPedazo, p);
            pedazo.GetComponent<SpriteRenderer>().color = color;
            pedazo.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(3f, 9f));
        }
    }

    public void CrearPolvo(Vector3 pos, int cantidad)
    {
        for (int i = 0; i < cantidad; i++)
        {
            float lado = (i % 2 == 0) ? 1f : -1f;
            GameObject polvo = Crear(prefabPolvo, pos + new Vector3(lado * 0.2f, 0f, 0f));
            polvo.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(lado * UnityEngine.Random.Range(1f, 2.5f), UnityEngine.Random.Range(0.3f, 1.2f));
        }
    }

    void ActualizarUI()
    {
        textoNivel.text = "FARO " + (nivelActual + 1) + ":  " + nombres[nivelActual];
        textoPista.text = pistas[nivelActual];
        textoMuertes.text = "MUERTES: " + muertes;

        for (int i = 0; i < cuadros.Length; i++)
        {
            if (i < nivelActual) cuadros[i].color = Color.white;
            else if (i == nivelActual) cuadros[i].color = colorLumi;
            else cuadros[i].color = new Color(1f, 1f, 1f, 0.2f);
        }
    }
}
