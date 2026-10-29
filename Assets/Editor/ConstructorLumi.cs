using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// Menu "Lumi > Construir juego": crea los sprites, animaciones, prefabs,
// la escena "Juego" y la interfaz. Se puede volver a ejecutar sin problema.
public static class ConstructorLumi
{
    const string SPR = "Assets/Sprites";
    const string PRE = "Assets/Prefabs";
    const string ANI = "Assets/Animaciones";
    const string ESC = "Assets/Escenas";
    const string MAT = "Assets/Materiales";

    static Sprite cuadrado;
    static Sprite triangulo;
    static Sprite circulo;
    static Material matSprite;
    static Font fuente;
    static AnimatorController ctrlLumi;

    static Color NAVY, HIELO, NARANJA, NARANJA_OSCURO, NEGRO, MORADO, BRUMA, AMARILLO, BLANCO, LUZ;

    static Color Hex(string h)
    {
        ColorUtility.TryParseHtmlString(h, out Color c);
        return c;
    }

    [MenuItem("Lumi/Construir juego")]
    public static void Construir()
    {
        NAVY = Hex("#1B2142");
        HIELO = Hex("#CFE3F0");
        NARANJA = Hex("#FF8A1F");
        NARANJA_OSCURO = Hex("#C25A08");
        NEGRO = Hex("#15151C");
        MORADO = Hex("#4B2A6B");
        BRUMA = Hex("#1A0E2E");
        AMARILLO = Hex("#FFD54A");
        BLANCO = Color.white;
        LUZ = new Color(1f, 0.85f, 0.45f, 0.22f);

        ctrlLumi = null;
        AssetDatabase.DeleteAsset(PRE);
        AssetDatabase.DeleteAsset(ANI);
        AssetDatabase.Refresh();
        foreach (string c in new[] { SPR, PRE, ANI, ESC, MAT })
            if (!AssetDatabase.IsValidFolder(c)) AssetDatabase.CreateFolder("Assets", c.Substring(7));

        cuadrado = CrearSprite("Cuadrado", false);
        triangulo = CrearSprite("Triangulo", true);
        circulo = CrearCirculo();
        matSprite = AssetDatabase.LoadAssetAtPath<Material>("Packages/com.unity.render-pipelines.universal/Runtime/Materials/Sprite-Unlit-Default.mat");
        fuente = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fuentes/FNAF.ttf");
        if (fuente == null) fuente = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        PhysicsMaterial2D sinFriccion = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(MAT + "/SinFriccion.physicsMaterial2D");
        if (sinFriccion == null)
        {
            sinFriccion = new PhysicsMaterial2D("SinFriccion") { friction = 0f, bounciness = 0f };
            AssetDatabase.CreateAsset(sinFriccion, MAT + "/SinFriccion.physicsMaterial2D");
        }

        var escena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // ---------- PREFABS ----------
        GameObject pLumi = Guardar(CrearLumi(sinFriccion), "Lumi");
        GameObject pNadie = Guardar(CrearNadieJugable(sinFriccion), "NadieJugable");
        GameObject pNadieComp = Guardar(CrearNadieCompanero(), "Nadie");
        GameObject pBruma = Guardar(CrearBruma(), "LaBruma");
        GameObject pAbuela = Guardar(CrearAbuela(), "AbuelaVela");
        GameObject pAldeano = Guardar(CrearAldeano(), "Aldeano");
        GameObject pSombra = Guardar(CrearSombra(), "Sombra");

        GameObject pBloque = Guardar(CrearBloque(), "Bloque");
        GameObject pFondo = Guardar(Cubo("Fondo", null, Vector2.zero, Vector2.one, HIELO, -10), "Fondo");
        GameObject pPincho = Guardar(CrearPincho(), "Pincho");
        GameObject pEspina = Guardar(CrearEspinaBruma(), "EspinaBruma");
        GameObject pGota = Guardar(CrearGota(), "Gota");
        GameObject pEscombro = Guardar(CrearEscombro(), "Escombro");
        GameObject pRoca = Guardar(CrearRoca(), "Roca");
        GameObject pEstalactita = Guardar(CrearEstalactita(), "Estalactita");
        GameObject pCascada = Guardar(CrearCascada(), "Cascada");
        GameObject pSeta = Guardar(CrearSeta(), "Seta");
        GameObject pAscensor = Guardar(CrearPlataforma(false), "Ascensor");
        GameObject pReloj = Guardar(CrearPlataforma(true), "Reloj");
        GameObject pCristal = Guardar(CrearCristal(), "Cristal");
        GameObject pFarol = Guardar(CrearFarol(), "Farol");
        GameObject pPilar = Guardar(CrearPilar(), "Pilar");
        GameObject pFaro = Guardar(CrearFaro(false), "Faro");
        GameObject pFaroFalso = Guardar(CrearFaro(true), "FaroFalso");
        GameObject pChispa = Guardar(CrearChispa(), "ChispaDeLumi");
        GameObject glitch = new GameObject("ElJuegoSeApaga");
        glitch.AddComponent<Glitch>();
        GameObject pGlitch = Guardar(glitch, "ElJuegoSeApaga");
        GameObject pLuciernaga = Guardar(CrearLuciernaga(), "Luciernaga");
        GameObject pPedazo = Guardar(CrearPedazo(), "Pedazo");
        GameObject pPolvo = Guardar(CrearPolvo(), "Polvo");

        // ---------- ESCENA ----------
        GameObject cam = new GameObject("Main Camera");
        cam.tag = "MainCamera";
        Camera c2 = cam.AddComponent<Camera>();
        c2.orthographic = true;
        c2.orthographicSize = 9f;
        c2.clearFlags = CameraClearFlags.SolidColor;
        c2.backgroundColor = Hex("#0B0F24");
        cam.transform.position = new Vector3(0f, 0f, -10f);
        cam.AddComponent<AudioListener>();

        Random.InitState(7);
        GameObject estrellas = new GameObject("Estrellas");
        for (int i = 0; i < 40; i++)
        {
            float x = Random.Range(-16f, 16f);
            float y = Random.Range(0, 2) == 0 ? Random.Range(4.3f, 8.8f) : Random.Range(-8.8f, -3.6f);
            float t = Random.Range(0.07f, 0.16f);
            Cubo("Estrella", estrellas.transform, new Vector2(x, y), new Vector2(t, t), new Color(1f, 1f, 1f, Random.Range(0.25f, 0.6f)), 1);
        }

        // ---------- UI ----------
        GameObject canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasGO.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler cs = canvasGO.GetComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080);
        cs.matchWidthOrHeight = 0.5f;
        Transform canvas = canvasGO.transform;
        Vector2 centro = new Vector2(0.5f, 0.5f);

        // HUD
        Transform hud = Contenedor(canvas, "HUD");
        Text tNivel = Texto(hud, "TextoNivel", "FARO 1", 40, BLANCO, TextAnchor.UpperLeft, new Vector2(0, 1), new Vector2(60, -45), new Vector2(1100, 70));
        Text tMuertes = Texto(hud, "TextoMuertes", "MUERTES: 0", 40, HIELO, TextAnchor.UpperRight, new Vector2(1, 1), new Vector2(-60, -45), new Vector2(600, 70));
        Text tPista = Texto(hud, "TextoPista", "", 36, AMARILLO, TextAnchor.MiddleCenter, new Vector2(0.5f, 1), new Vector2(0, -178), new Vector2(1800, 80));
        Text tDialogo = Texto(hud, "TextoDialogo", "", 36, BLANCO, TextAnchor.MiddleCenter, new Vector2(0.5f, 0), new Vector2(0, 190), new Vector2(1800, 60));
        Texto(hud, "TextoControles", "A/D: moverse     ESPACIO: saltar     Q: contener luz     SHIFT: destello     R: reiniciar", 26, new Color(1, 1, 1, 0.55f), TextAnchor.MiddleCenter, new Vector2(0.5f, 0), new Vector2(0, 70), new Vector2(1800, 60));
        Text tAviso = Texto(hud, "TextoAviso", "", 70, AMARILLO, TextAnchor.MiddleCenter, centro, new Vector2(0, 170), new Vector2(1600, 120));
        Image[] cuadros = new Image[10];
        for (int i = 0; i < cuadros.Length; i++)
            cuadros[i] = Imagen(hud, "Cuadro" + (i + 1), new Vector2(0.5f, 1), new Vector2((i - 4.5f) * 38f, -122), new Vector2(26, 26), BLANCO);

        // Falla (nivel 8)
        GameObject panelFalla = Imagen(canvas, "PanelFalla", centro, Vector2.zero, new Vector2(4000, 3000), new Color(0, 0, 0, 0.85f)).gameObject;
        Texto(panelFalla.transform, "TextoFalla", "ERR0R  ·  99:59:59", 70, Hex("#FF3B3B"), TextAnchor.MiddleCenter, centro, Vector2.zero, new Vector2(1600, 120));

        // Titulo
        GameObject panelTitulo = Imagen(canvas, "PanelTitulo", centro, Vector2.zero, new Vector2(4000, 3000), NAVY).gameObject;
        Transform pt = panelTitulo.transform;
        Texto(pt, "Titulo", "LUMI", 170, NARANJA, TextAnchor.MiddleCenter, centro, new Vector2(0, 370), new Vector2(1600, 200));
        LumiUI(pt, new Vector2(0, 160), NARANJA, NARANJA_OSCURO, NEGRO, 2);
        Texto(pt, "Subtitulo", "EL VALLE DETENIDO", 64, BLANCO, TextAnchor.MiddleCenter, centro, new Vector2(0, -20), new Vector2(1600, 90));
        Texto(pt, "Lema", "La oscuridad no mata. Congela el tiempo.", 34, HIELO, TextAnchor.MiddleCenter, centro, new Vector2(0, -100), new Vector2(1700, 60));
        Text tEmpezar = Texto(pt, "NuevaPartida", "> NUEVA PARTIDA", 46, AMARILLO, TextAnchor.MiddleCenter, centro, new Vector2(0, -210), new Vector2(1600, 70));
        Texto(pt, "PartidaBloqueada", "LUMI · DÍA ETERNO · 99:59:59      [BLOQUEADA]", 30, new Color(1, 1, 1, 0.3f), TextAnchor.MiddleCenter, centro, new Vector2(0, -285), new Vector2(1600, 60));
        Texto(pt, "Controles", "A/D: moverse     ESPACIO: saltar     Q: contener luz     SHIFT: destello", 26, new Color(1, 1, 1, 0.5f), TextAnchor.MiddleCenter, centro, new Vector2(0, -400), new Vector2(1800, 60));
        Texto(pt, "Creditos", "Camila Urquizo  ·  Israel Pilla", 24, new Color(1, 1, 1, 0.4f), TextAnchor.MiddleCenter, centro, new Vector2(0, -470), new Vector2(1600, 50));

        // Eventos de historia
        GameObject panelEvento = Imagen(canvas, "PanelEvento", centro, Vector2.zero, new Vector2(4000, 3000), new Color(NAVY.r, NAVY.g, NAVY.b, 0.98f)).gameObject;
        Text tEvTitulo = Texto(panelEvento.transform, "EventoTitulo", "", 60, AMARILLO, TextAnchor.MiddleCenter, centro, new Vector2(0, 360), new Vector2(1700, 100));
        tEvTitulo.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        Text tEvCuerpo = Texto(panelEvento.transform, "EventoCuerpo", "", 36, BLANCO, TextAnchor.UpperCenter, centro, new Vector2(0, 230), new Vector2(1700, 560));
        tEvCuerpo.rectTransform.pivot = new Vector2(0.5f, 1f);
        tEvCuerpo.rectTransform.anchoredPosition = new Vector2(0, 270);
        tEvCuerpo.verticalOverflow = VerticalWrapMode.Overflow;
        tEvCuerpo.lineSpacing = 1.1f;
        Texto(panelEvento.transform, "EventoContinuar", "ESPACIO para continuar", 28, new Color(1, 1, 1, 0.45f), TextAnchor.MiddleCenter, centro, new Vector2(0, -470), new Vector2(1000, 50));

        // ERROR (plot twist)
        GameObject panelError = Imagen(canvas, "PanelError", centro, Vector2.zero, new Vector2(4000, 3000), Color.black).gameObject;
        Text tError = Texto(panelError.transform, "TextoError", "ERROR\n<size=56>ESTA PARTIDA YA EXISTÍA</size>", 130, Hex("#FF3B3B"), TextAnchor.MiddleCenter, centro, Vector2.zero, new Vector2(1800, 400));

        // Final
        GameObject panelFinal = Imagen(canvas, "PanelFinal", centro, Vector2.zero, new Vector2(4000, 3000), Hex("#120A1E")).gameObject;
        LumiUI(panelFinal.transform, new Vector2(0, 300), Hex("#2A1640"), Hex("#1A0E2E"), Hex("#B04BE0"), 1);
        Text tFinal = Texto(panelFinal.transform, "TextoFinal", "", 48, BLANCO, TextAnchor.MiddleCenter, centro, new Vector2(0, -150), new Vector2(1700, 450));

        // ---------- GAME MANAGER ----------
        GameObject gmGO = new GameObject("GameManager");
        AudioSource fuenteMusica = gmGO.AddComponent<AudioSource>();
        fuenteMusica.playOnAwake = false;
        fuenteMusica.loop = true;
        fuenteMusica.volume = 0.45f;
        GameManager gm = gmGO.AddComponent<GameManager>();
        GameObject efectosGO = new GameObject("Efectos");
        efectosGO.transform.SetParent(gmGO.transform, false);
        AudioSource fuenteEfectos = efectosGO.AddComponent<AudioSource>();
        fuenteEfectos.playOnAwake = false;
        fuenteEfectos.volume = 0.8f;
        gm.fuenteEfectos = fuenteEfectos;
        var listaEfectos = new System.Collections.Generic.List<AudioClip>();
        foreach (string guid in AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets/Sonidos" }))
            listaEfectos.Add(AssetDatabase.LoadAssetAtPath<AudioClip>(AssetDatabase.GUIDToAssetPath(guid)));
        gm.efectos = listaEfectos.ToArray();
        gm.musicaValle = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Musica/ValleDetenido.wav");
        gm.musicaBruma = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Musica/LaBruma.wav");
        gm.prefabLumi = pLumi;
        gm.prefabNadieJugable = pNadie;
        gm.prefabNadieCompanero = pNadieComp;
        gm.prefabBruma = pBruma;
        gm.prefabAbuelaVela = pAbuela;
        gm.prefabAldeano = pAldeano;
        gm.prefabSombra = pSombra;
        gm.prefabBloque = pBloque;
        gm.prefabFondo = pFondo;
        gm.prefabPincho = pPincho;
        gm.prefabEspinaBruma = pEspina;
        gm.prefabGota = pGota;
        gm.prefabEscombro = pEscombro;
        gm.prefabRoca = pRoca;
        gm.prefabEstalactita = pEstalactita;
        gm.prefabCascada = pCascada;
        gm.prefabSeta = pSeta;
        gm.prefabAscensor = pAscensor;
        gm.prefabReloj = pReloj;
        gm.prefabCristal = pCristal;
        gm.prefabFarol = pFarol;
        gm.prefabPilar = pPilar;
        gm.prefabFaro = pFaro;
        gm.prefabFaroFalso = pFaroFalso;
        gm.prefabChispa = pChispa;
        gm.prefabGlitch = pGlitch;
        gm.prefabLuciernaga = pLuciernaga;
        gm.prefabPedazo = pPedazo;
        gm.prefabPolvo = pPolvo;
        gm.hud = hud.gameObject;
        gm.panelTitulo = panelTitulo;
        gm.panelEvento = panelEvento;
        gm.panelError = panelError;
        gm.panelFalla = panelFalla;
        gm.panelFinal = panelFinal;
        gm.textoNivel = tNivel;
        gm.textoPista = tPista;
        gm.textoMuertes = tMuertes;
        gm.textoAviso = tAviso;
        gm.textoDialogo = tDialogo;
        gm.textoEmpezar = tEmpezar;
        gm.textoEventoTitulo = tEvTitulo;
        gm.textoEventoCuerpo = tEvCuerpo;
        gm.textoError = tError;
        gm.textoFinal = tFinal;
        gm.cuadros = cuadros;
        gm.colorLumi = NARANJA;
        gm.coloresMundo = new[] { Hex("#3E6478"), Hex("#4E4682"), Hex("#5E5470"), Hex("#3A2E5C") };

        string rutaEscena = ESC + "/Juego.unity";
        EditorSceneManager.SaveScene(escena, rutaEscena);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(rutaEscena, true) };

        PlayerSettings.productName = "Lumi El Valle Detenido";
        PlayerSettings.defaultWebScreenWidth = 960;
        PlayerSettings.defaultWebScreenHeight = 540;
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
        PlayerSettings.WebGL.decompressionFallback = true;

        AssetDatabase.SaveAssets();
        Debug.Log("LUMI: juego construido. Abre Assets/Escenas/Juego y presiona Play.");
    }

    // ================= PERSONAJES =================

    static GameObject CuerpoJugable(string nombre, PhysicsMaterial2D sinFriccion, out Transform visual)
    {
        GameObject g = new GameObject(nombre);
        Rigidbody2D rb = g.AddComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        BoxCollider2D col = g.AddComponent<BoxCollider2D>();
        col.size = new Vector2(0.72f, 0.86f);
        col.edgeRadius = 0.02f;
        col.sharedMaterial = sinFriccion;
        visual = Vacio("Visual", g.transform, Vector2.zero);
        return g;
    }

    static GameObject CrearLumi(PhysicsMaterial2D sinFriccion)
    {
        GameObject lumi = CuerpoJugable("Lumi", sinFriccion, out Transform visual);
        Transform torso = Vacio("Torso", visual, new Vector2(0f, 0.1f));
        Cubo("Cuerpo", torso, Vector2.zero, new Vector2(0.8f, 0.7f), NARANJA, 5);
        Cubo("Sombreado", torso, new Vector2(0f, -0.31f), new Vector2(0.8f, 0.08f), NARANJA_OSCURO, 6);
        Cubo("Brillo", torso, new Vector2(-0.26f, 0.22f), new Vector2(0.12f, 0.12f), Hex("#FFC27A"), 6);
        Transform ojos = Vacio("Ojos", torso, new Vector2(0.14f, 0.08f));
        Cubo("OjoIzq", ojos, new Vector2(-0.11f, 0f), new Vector2(0.12f, 0.2f), NEGRO, 6);
        Cubo("OjoDer", ojos, new Vector2(0.11f, 0f), new Vector2(0.12f, 0.2f), NEGRO, 6);
        Cubo("BrilloIzq", ojos, new Vector2(-0.09f, 0.05f), new Vector2(0.04f, 0.05f), BLANCO, 7);
        Cubo("BrilloDer", ojos, new Vector2(0.13f, 0.05f), new Vector2(0.04f, 0.05f), BLANCO, 7);
        Cubo("Boca", torso, new Vector2(0.14f, -0.14f), new Vector2(0.16f, 0.05f), NEGRO, 6);
        Transform antena = Vacio("Antena", torso, new Vector2(0f, 0.35f));
        Llama(antena, Hex("#FF5A1F"), AMARILLO);
        Cubo("PiernaIzq", visual, new Vector2(-0.18f, -0.35f), new Vector2(0.16f, 0.2f), NARANJA_OSCURO, 4);
        Cubo("PiernaDer", visual, new Vector2(0.18f, -0.35f), new Vector2(0.16f, 0.2f), NARANJA_OSCURO, 4);

        // Su luz
        FuenteLuz luz = lumi.AddComponent<FuenteLuz>();
        luz.radio = 2.6f;
        luz.halo = Halo(lumi.transform, LUZ, 5.2f).GetComponent<SpriteRenderer>();

        Lumi script = lumi.AddComponent<Lumi>();
        script.visual = visual;
        lumi.AddComponent<Animator>().runtimeAnimatorController = ControladorLumi();
        return lumi;
    }

    static GameObject CrearNadieJugable(PhysicsMaterial2D sinFriccion)
    {
        Color cuerpo = Hex("#1E2240");
        GameObject n = CuerpoJugable("NadieJugable", sinFriccion, out Transform visual);
        Transform torso = Vacio("Torso", visual, new Vector2(0f, 0.1f));
        Cubo("Cuerpo", torso, Vector2.zero, new Vector2(0.8f, 0.7f), cuerpo, 5);
        Transform ojos = Vacio("Ojos", torso, new Vector2(0.1f, 0.06f));
        Cubo("Ojo", ojos, Vector2.zero, new Vector2(0.34f, 0.34f), BLANCO, 6);
        Cubo("Pupila", ojos, new Vector2(0.06f, 0f), new Vector2(0.15f, 0.15f), NEGRO, 7);
        Transform antena = Vacio("Antena", torso, new Vector2(0f, 0.35f));
        Cubo("PiernaIzq", visual, new Vector2(-0.18f, -0.35f), new Vector2(0.16f, 0.2f), Hex("#14172E"), 4);
        Cubo("PiernaDer", visual, new Vector2(0.18f, -0.35f), new Vector2(0.16f, 0.2f), Hex("#14172E"), 4);

        Lumi script = n.AddComponent<Lumi>();
        script.visual = visual;
        script.esNadie = true;
        n.AddComponent<Animator>().runtimeAnimatorController = ControladorLumi();
        return n;
    }

    static GameObject CrearNadieCompanero()
    {
        GameObject n = new GameObject("Nadie");
        Color cuerpo = Hex("#1A1E3C");
        Cubo("Cuerpo", n.transform, Vector2.zero, new Vector2(0.5f, 0.5f), cuerpo, 8);
        Cubo("Fleco1", n.transform, new Vector2(-0.15f, -0.3f), new Vector2(0.12f, 0.12f), cuerpo, 8);
        Cubo("Fleco2", n.transform, new Vector2(0.15f, -0.3f), new Vector2(0.12f, 0.12f), cuerpo, 8);
        Cubo("Ojo", n.transform, new Vector2(0f, 0.04f), new Vector2(0.24f, 0.24f), BLANCO, 9);
        GameObject pupila = Cubo("Pupila", n.transform, new Vector2(0f, 0.04f), new Vector2(0.11f, 0.11f), NEGRO, 10);
        n.AddComponent<NadieCompanero>().pupila = pupila.transform;
        return n;
    }

    static GameObject CrearBruma()
    {
        GameObject b = new GameObject("LaBruma");
        Rigidbody2D rb = b.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        BoxCollider2D col = b.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(0.65f, 0.8f);

        Transform visual = Vacio("Visual", b.transform, Vector2.zero);
        Halo(visual, new Color(0.45f, 0.15f, 0.7f, 0.35f), 2.6f);
        Transform torso = Vacio("Torso", visual, new Vector2(0f, 0.1f));
        Cubo("Cuerpo", torso, Vector2.zero, new Vector2(0.8f, 0.7f), BRUMA, 5);
        Transform ojos = Vacio("Ojos", torso, new Vector2(0.14f, 0.08f));
        Cubo("OjoIzq", ojos, new Vector2(-0.11f, 0f), new Vector2(0.12f, 0.2f), Hex("#3A1E5C"), 6);   // el ojo que le falta
        Cubo("OjoDer", ojos, new Vector2(0.11f, 0f), new Vector2(0.12f, 0.2f), Hex("#E05BFF"), 6);
        Cubo("Boca", torso, new Vector2(0.14f, -0.14f), new Vector2(0.16f, 0.05f), Hex("#3A1E5C"), 6);
        Transform antena = Vacio("Antena", torso, new Vector2(0f, 0.35f));
        Llama(antena, Hex("#4A1F7A"), Hex("#B04BE0"));
        Cubo("PiernaIzq", visual, new Vector2(-0.18f, -0.35f), new Vector2(0.16f, 0.2f), BRUMA, 4);
        Cubo("PiernaDer", visual, new Vector2(0.18f, -0.35f), new Vector2(0.16f, 0.2f), BRUMA, 4);

        b.AddComponent<Bruma>().visual = visual;
        return b;
    }

    static GameObject CrearAbuela()
    {
        GameObject a = new GameObject("AbuelaVela");
        Cubo("Cuerpo", a.transform, new Vector2(0f, -0.07f), new Vector2(0.6f, 0.86f), Hex("#F3E6C4"), 2);
        Cubo("Chal", a.transform, new Vector2(0f, -0.12f), new Vector2(0.66f, 0.18f), Hex("#7B4A9E"), 3);
        Cubo("OjoIzq", a.transform, new Vector2(-0.1f, 0.18f), new Vector2(0.12f, 0.03f), NEGRO, 3);
        Cubo("OjoDer", a.transform, new Vector2(0.1f, 0.18f), new Vector2(0.12f, 0.03f), NEGRO, 3);
        Cubo("Mecha", a.transform, new Vector2(0f, 0.42f), new Vector2(0.04f, 0.1f), NEGRO, 2);
        GameObject llama = Cubo("Llama", a.transform, new Vector2(0f, 0.58f), new Vector2(0.22f, 0.3f), Hex("#FF9A2E"), 3, triangulo);
        a.AddComponent<Congelable>();
        a.AddComponent<AbuelaVela>();
        AnimationClip c = Clip("Abuela_Llama", 0.5f, true);
        Escala(c, "Llama", new[] { 0f, 0.25f, 0.5f }, new[] { 0.22f, 0.18f, 0.22f }, new[] { 0.3f, 0.36f, 0.3f });
        a.AddComponent<Animator>().runtimeAnimatorController = ControladorSimple("AbuelaVela", c);
        return a;
    }

    static GameObject CrearAldeano()
    {
        // Un nino congelado a mitad de un salto
        GameObject k = new GameObject("Aldeano");
        Rigidbody2D rb = k.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        k.AddComponent<BoxCollider2D>().size = new Vector2(0.8f, 0.9f);
        Cubo("Cabeza", k.transform, new Vector2(0f, 0.22f), new Vector2(0.45f, 0.42f), Hex("#F2C79A"), 3);
        Cubo("Ojo1", k.transform, new Vector2(0.04f, 0.24f), new Vector2(0.06f, 0.1f), NEGRO, 4);
        Cubo("Ojo2", k.transform, new Vector2(0.16f, 0.24f), new Vector2(0.06f, 0.1f), NEGRO, 4);
        Cubo("Camisa", k.transform, new Vector2(0f, -0.13f), new Vector2(0.55f, 0.32f), Hex("#4CAF7A"), 3);
        GameObject b1 = Cubo("BrazoIzq", k.transform, new Vector2(-0.34f, 0.05f), new Vector2(0.1f, 0.3f), Hex("#4CAF7A"), 2);
        b1.transform.localRotation = Quaternion.Euler(0, 0, 30);
        GameObject b2 = Cubo("BrazoDer", k.transform, new Vector2(0.34f, 0.05f), new Vector2(0.1f, 0.3f), Hex("#4CAF7A"), 2);
        b2.transform.localRotation = Quaternion.Euler(0, 0, -30);
        GameObject p1 = Cubo("PiernaIzq", k.transform, new Vector2(-0.14f, -0.38f), new Vector2(0.14f, 0.2f), Hex("#3B4C7A"), 2);
        p1.transform.localRotation = Quaternion.Euler(0, 0, -35);
        Cubo("PiernaDer", k.transform, new Vector2(0.14f, -0.38f), new Vector2(0.14f, 0.2f), Hex("#3B4C7A"), 2);
        k.AddComponent<Congelable>();
        ObjetoCongelado o = k.AddComponent<ObjetoCongelado>();
        o.modo = ObjetoCongelado.Modo.Atravesar;
        return k;
    }

    static GameObject CrearSombra()
    {
        GameObject s = new GameObject("Sombra");
        Rigidbody2D rb = s.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        BoxCollider2D col = s.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(0.75f, 0.8f);
        s.AddComponent<Peligro>();

        GameObject solido = new GameObject("Solido");
        solido.transform.SetParent(s.transform, false);
        BoxCollider2D cs = solido.AddComponent<BoxCollider2D>();
        cs.size = new Vector2(0.85f, 0.85f);

        Transform visual = Vacio("Visual", s.transform, Vector2.zero);
        Cubo("Cuerpo", visual, Vector2.zero, new Vector2(0.9f, 0.9f), MORADO, 2);
        Cubo("CuernoIzq", visual, new Vector2(-0.3f, 0.55f), new Vector2(0.25f, 0.25f), MORADO, 2, triangulo);
        Cubo("CuernoDer", visual, new Vector2(0.3f, 0.55f), new Vector2(0.25f, 0.25f), MORADO, 2, triangulo);
        Cubo("Ojo", visual, new Vector2(0f, 0.08f), new Vector2(0.46f, 0.46f), BLANCO, 3);
        GameObject pupila = Cubo("Pupila", visual, new Vector2(0f, 0.08f), new Vector2(0.2f, 0.2f), NEGRO, 4);
        Cubo("Boca", visual, new Vector2(0f, -0.28f), new Vector2(0.4f, 0.06f), NEGRO, 3);

        s.AddComponent<Congelable>();
        Sombra sc = s.AddComponent<Sombra>();
        sc.pupila = pupila.transform;
        sc.cuerpoSolido = cs;

        AnimationClip flotar = Clip("Sombra_Flotar", 0.8f, true);
        Pos(flotar, "Visual", new[] { 0f, 0.4f, 0.8f }, new[] { 0f, 0f, 0f }, new[] { 0f, 0.1f, 0f });
        Escala(flotar, "Visual", new[] { 0f, 0.4f, 0.8f }, new[] { 1f, 1.06f, 1f }, new[] { 1f, 0.95f, 1f });
        s.AddComponent<Animator>().runtimeAnimatorController = ControladorSimple("Sombra", flotar);
        return s;
    }

    // ================= ESCENARIO =================

    static GameObject CrearBloque()
    {
        GameObject b = Cubo("Bloque", null, Vector2.zero, Vector2.one, NAVY, 0);
        b.AddComponent<BoxCollider2D>();
        b.AddComponent<Bloque>();
        return b;
    }

    static GameObject CrearPincho()
    {
        GameObject p = new GameObject("Pincho");
        Cubo("Punta", p.transform, new Vector2(0f, -0.1f), new Vector2(0.9f, 0.8f), Hex("#0E1230"), 1, triangulo);
        BoxCollider2D col = p.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(0.5f, 0.45f);
        col.offset = new Vector2(0f, -0.25f);
        p.AddComponent<Peligro>();
        return p;
    }

    static GameObject CrearEspinaBruma()
    {
        GameObject p = new GameObject("EspinaBruma");
        Cubo("Punta", p.transform, new Vector2(0f, -0.1f), new Vector2(0.9f, 0.8f), Hex("#6A2FA0"), -1, triangulo);
        BoxCollider2D col = p.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(0.5f, 0.45f);
        col.offset = new Vector2(0f, -0.25f);
        p.AddComponent<Peligro>();
        Congelable c = p.AddComponent<Congelable>();
        c.celdaFija = true;
        c.cambiarColor = false;
        p.AddComponent<EspinaBruma>();
        return p;
    }

    static GameObject ObjetoQueCae(string nombre, Vector2 tamColision, bool trigger)
    {
        GameObject g = new GameObject(nombre);
        Rigidbody2D rb = g.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.freezeRotation = true;
        BoxCollider2D col = g.AddComponent<BoxCollider2D>();
        col.size = tamColision;
        col.isTrigger = trigger;
        return g;
    }

    static GameObject CrearGota()
    {
        GameObject g = ObjetoQueCae("Gota", new Vector2(0.8f, 0.35f), false);
        Color agua = Hex("#3D8BFF");
        Cubo("Agua", g.transform, Vector2.zero, new Vector2(0.8f, 0.35f), agua, 3);
        Cubo("Brillo", g.transform, new Vector2(-0.22f, 0.07f), new Vector2(0.2f, 0.08f), BLANCO, 4);
        GameObject punta = Cubo("Punta", g.transform, new Vector2(0f, -0.25f), new Vector2(0.25f, 0.18f), agua, 3, triangulo);
        punta.transform.localRotation = Quaternion.Euler(0, 0, 180);
        g.AddComponent<Congelable>();
        g.AddComponent<ObjetoCongelado>().colorPedazos = agua;
        return g;
    }

    static GameObject CrearEscombro()
    {
        GameObject g = ObjetoQueCae("Escombro", Vector2.one, false);
        Color tierra = Hex("#8A6A4E");
        Cubo("Piedra", g.transform, Vector2.zero, Vector2.one, tierra, 3);
        Cubo("Borde", g.transform, new Vector2(0f, 0.42f), new Vector2(1f, 0.16f), Hex("#A88664"), 4);
        Cubo("Grieta1", g.transform, new Vector2(-0.15f, 0f), new Vector2(0.06f, 0.4f), Hex("#5E4632"), 4);
        Cubo("Grieta2", g.transform, new Vector2(0.2f, -0.2f), new Vector2(0.3f, 0.06f), Hex("#5E4632"), 4);
        g.AddComponent<Congelable>();
        g.AddComponent<ObjetoCongelado>().colorPedazos = tierra;
        return g;
    }

    static GameObject CrearRoca()
    {
        GameObject g = ObjetoQueCae("Roca", new Vector2(0.95f, 0.95f), false);
        Cubo("Piedra", g.transform, Vector2.zero, new Vector2(0.95f, 0.95f), Hex("#6E6258"), 3);
        Cubo("Brillo", g.transform, new Vector2(-0.2f, 0.25f), new Vector2(0.25f, 0.15f), Hex("#8E8278"), 4);
        g.AddComponent<Congelable>();
        ObjetoCongelado o = g.AddComponent<ObjetoCongelado>();
        o.seRompeAlCaer = false;
        o.aplasta = true;
        return g;
    }

    static GameObject CrearEstalactita()
    {
        GameObject g = ObjetoQueCae("Estalactita", new Vector2(0.35f, 0.7f), true);
        g.AddComponent<Peligro>();
        GameObject hielo = Cubo("Hielo", g.transform, Vector2.zero, new Vector2(0.6f, 0.9f), Hex("#5E86B8"), 3, triangulo);
        hielo.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
        g.AddComponent<Congelable>();
        ObjetoCongelado o = g.AddComponent<ObjetoCongelado>();
        o.peligrosoAlCaer = true;
        o.colorPedazos = Hex("#9CC8F0");
        return g;
    }

    static GameObject CrearCascada()
    {
        GameObject g = new GameObject("Cascada");
        g.AddComponent<BoxCollider2D>();
        Cubo("Agua", g.transform, Vector2.zero, Vector2.one, Hex("#3FA0E0"), 2);
        Cubo("Chorro1", g.transform, new Vector2(-0.25f, 0f), new Vector2(0.08f, 0.9f), Hex("#9AD4FF"), 3);
        Cubo("Chorro2", g.transform, new Vector2(0.2f, 0.1f), new Vector2(0.08f, 0.7f), Hex("#9AD4FF"), 3);
        g.AddComponent<Congelable>();
        ObjetoCongelado o = g.AddComponent<ObjetoCongelado>();
        o.modo = ObjetoCongelado.Modo.Atravesar;
        return g;
    }

    static GameObject CrearSeta()
    {
        GameObject s = new GameObject("Seta");
        BoxCollider2D col = s.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(0.6f, 0.5f);
        col.offset = new Vector2(0f, -0.25f);
        Cubo("Tallo", s.transform, new Vector2(0f, -0.32f), new Vector2(0.25f, 0.35f), Hex("#F3E6C4"), 2);
        Transform sombrero = Vacio("Sombrero", s.transform, new Vector2(0f, -0.12f));
        Cubo("Gorro", sombrero, new Vector2(0f, 0.05f), new Vector2(0.8f, 0.3f), Hex("#E0503A"), 3);
        Cubo("Punto1", sombrero, new Vector2(-0.2f, 0.08f), new Vector2(0.1f, 0.1f), BLANCO, 4);
        Cubo("Punto2", sombrero, new Vector2(0.15f, 0.12f), new Vector2(0.1f, 0.1f), BLANCO, 4);
        s.AddComponent<Congelable>();
        s.AddComponent<Seta>().sombrero = sombrero;
        return s;
    }

    static GameObject CrearPlataforma(bool esReloj)
    {
        GameObject p = new GameObject(esReloj ? "Reloj" : "Ascensor");
        Rigidbody2D rb = p.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        p.AddComponent<BoxCollider2D>().size = new Vector2(2f, 0.4f);
        Cubo("Tabla", p.transform, Vector2.zero, new Vector2(2f, 0.4f), esReloj ? Hex("#C9A15A") : Hex("#9C6B3F"), 3);
        Cubo("Borde", p.transform, new Vector2(0f, 0.15f), new Vector2(2f, 0.1f), esReloj ? Hex("#E8C77E") : Hex("#B9855A"), 4);
        PlataformaMovil pm = p.AddComponent<PlataformaMovil>();
        if (esReloj)
        {
            Cubo("Esfera", p.transform, new Vector2(0f, -0.45f), new Vector2(0.6f, 0.6f), Hex("#F3E6C4"), 3);
            Transform m = Vacio("Manecilla", p.transform, new Vector2(0f, -0.45f));
            Cubo("Aguja", m, new Vector2(0f, 0.12f), new Vector2(0.05f, 0.24f), NEGRO, 4);
            pm.manecilla = m;
        }
        else
        {
            Cubo("Cadena1", p.transform, new Vector2(-0.8f, 0.8f), new Vector2(0.05f, 1.2f), NAVY, 2);
            Cubo("Cadena2", p.transform, new Vector2(0.8f, 0.8f), new Vector2(0.05f, 1.2f), NAVY, 2);
        }
        p.AddComponent<Congelable>();
        return p;
    }

    static GameObject CrearCristal()
    {
        GameObject c = new GameObject("Cristal");
        GameObject piedra = Cubo("Piedra", c.transform, Vector2.zero, new Vector2(0.5f, 0.5f), Hex("#5FE3E8"), 3);
        piedra.transform.localRotation = Quaternion.Euler(0, 0, 45);
        Cubo("Brillo", c.transform, new Vector2(-0.06f, 0.08f), new Vector2(0.1f, 0.1f), BLANCO, 4);
        GameObject rayo = Cubo("Rayo", null, Vector2.zero, Vector2.one, new Color(0.6f, 1f, 1f, 0.6f), 6);
        rayo.transform.SetParent(c.transform, false);
        c.AddComponent<Congelable>();
        c.AddComponent<Cristal>().rayo = rayo.transform;
        return c;
    }

    static GameObject CrearFarol()
    {
        GameObject f = new GameObject("Farol");
        Cubo("Cadena", f.transform, new Vector2(0f, 1.2f), new Vector2(0.05f, 2f), NAVY, 1);
        Cubo("Marco", f.transform, Vector2.zero, new Vector2(0.4f, 0.5f), NAVY, 2);
        Cubo("Fuego", f.transform, Vector2.zero, new Vector2(0.24f, 0.3f), AMARILLO, 3);
        FuenteLuz luz = f.AddComponent<FuenteLuz>();
        luz.radio = 1.1f;
        luz.halo = Halo(f.transform, new Color(1f, 0.8f, 0.4f, 0.28f), 2.2f).GetComponent<SpriteRenderer>();
        return f;
    }

    static GameObject CrearPilar()
    {
        GameObject p = new GameObject("Pilar");
        BoxCollider2D col = p.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(0.8f, 1.6f);
        col.offset = new Vector2(0f, 0.8f);
        Cubo("Piedra", p.transform, new Vector2(0f, 0.7f), new Vector2(0.6f, 1.4f), Hex("#5A5470"), 2);
        Cubo("Brasero", p.transform, new Vector2(0f, 1.45f), new Vector2(0.8f, 0.2f), NAVY, 3);
        GameObject llama = Cubo("Llama", p.transform, new Vector2(0f, 1.8f), new Vector2(0.5f, 0.6f), AMARILLO, 3, triangulo);
        FuenteLuz luz = p.AddComponent<FuenteLuz>();
        luz.radio = 1.8f;
        luz.halo = Halo(p.transform, new Color(1f, 0.8f, 0.4f, 0.28f), 0f).GetComponent<SpriteRenderer>();
        luz.halo.transform.localPosition = new Vector3(0f, 1.5f, 0f);
        Pilar pi = p.AddComponent<Pilar>();
        pi.llama = llama.GetComponent<SpriteRenderer>();
        pi.luz = luz;
        return p;
    }

    static GameObject CrearFaro(bool falso)
    {
        GameObject p = new GameObject(falso ? "FaroFalso" : "Faro");
        BoxCollider2D col = p.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(0.6f, 1.2f);
        col.offset = new Vector2(0f, 0.7f);

        Cubo("Marco", p.transform, new Vector2(0f, 0.85f), new Vector2(1.1f, 1.7f), NAVY, 1);
        GameObject luz = Cubo("Luz", p.transform, new Vector2(0f, 0.75f), new Vector2(0.7f, 1.3f), AMARILLO, 2);
        Cubo("Techo", p.transform, new Vector2(0f, 1.85f), new Vector2(0.6f, 0.4f), NAVY, 1, triangulo);
        // Quemaduras de destello (una pista del giro)
        Cubo("Quemadura1", p.transform, new Vector2(0.45f, 1.3f), new Vector2(0.14f, 0.2f), Hex("#0B0F24"), 3);
        Cubo("Quemadura2", p.transform, new Vector2(-0.44f, 0.35f), new Vector2(0.12f, 0.16f), Hex("#0B0F24"), 3);

        if (falso)
        {
            p.AddComponent<Peligro>();
            Transform ojo = Vacio("Ojo", p.transform, new Vector2(0f, 0.9f));
            Cubo("Blanco", ojo, Vector2.zero, new Vector2(0.4f, 0.4f), BLANCO, 3);
            Cubo("Pupila", ojo, Vector2.zero, new Vector2(0.18f, 0.18f), NEGRO, 4);
            FaroFalso ff = p.AddComponent<FaroFalso>();
            ff.luz = luz.GetComponent<SpriteRenderer>();
            ff.ojo = ojo.gameObject;
        }
        else
        {
            p.AddComponent<PuertaFaro>().luz = luz.GetComponent<SpriteRenderer>();
        }

        AnimationClip brillar = Clip("Faro_Brillar", 1f, true);
        Escala(brillar, "Luz", new[] { 0f, 0.5f, 1f }, new[] { 0.7f, 0.58f, 0.7f }, new[] { 1.3f, 1.18f, 1.3f });
        p.AddComponent<Animator>().runtimeAnimatorController = ControladorSimple("Faro", brillar);
        return p;
    }

    static GameObject CrearChispa()
    {
        GameObject c = new GameObject("ChispaDeLumi");
        BoxCollider2D col = c.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(0.6f, 0.6f);
        col.offset = new Vector2(0f, 0.6f);
        Halo(c.transform, new Color(1f, 0.6f, 0.2f, 0.3f), 1.4f).transform.localPosition = new Vector3(0f, 0.6f, 0f);
        GameObject d = Cubo("Diamante", c.transform, new Vector2(0f, 0.6f), new Vector2(0.45f, 0.45f), NARANJA, 3);
        d.transform.localRotation = Quaternion.Euler(0, 0, 45);
        Cubo("Luz", c.transform, new Vector2(0f, 0.6f), new Vector2(0.2f, 0.2f), AMARILLO, 4);
        c.AddComponent<PuertaFaro>();
        AnimationClip pulso = Clip("Chispa_Pulso", 0.6f, true);
        Escala(pulso, "Luz", new[] { 0f, 0.3f, 0.6f }, new[] { 0.2f, 0.32f, 0.2f }, new[] { 0.2f, 0.32f, 0.2f });
        c.AddComponent<Animator>().runtimeAnimatorController = ControladorSimple("Chispa", pulso);
        return c;
    }

    static GameObject CrearLuciernaga()
    {
        GameObject l = new GameObject("Luciernaga");
        Halo(l.transform, new Color(1f, 0.95f, 0.5f, 0.3f), 0.6f);
        Cubo("Cuerpo", l.transform, Vector2.zero, new Vector2(0.12f, 0.12f), Hex("#FFE97A"), 4);
        l.AddComponent<Congelable>();
        l.AddComponent<Luciernaga>();
        return l;
    }

    static GameObject CrearPedazo()
    {
        GameObject p = Cubo("Pedazo", null, Vector2.zero, new Vector2(0.18f, 0.18f), NARANJA, 7);
        p.AddComponent<Rigidbody2D>().gravityScale = 2f;
        p.AddComponent<AutoDestruir>().tiempo = 1.2f;
        return p;
    }

    static GameObject CrearPolvo()
    {
        GameObject p = Cubo("Polvo", null, Vector2.zero, new Vector2(0.12f, 0.12f), new Color(1f, 1f, 1f, 0.75f), 6);
        p.AddComponent<Rigidbody2D>().gravityScale = 0f;
        p.AddComponent<AutoDestruir>().tiempo = 0.35f;
        return p;
    }

    // La llamita de Lumi (Lumi es una chispa viviente, no un robot)
    static void Llama(Transform padre, Color fuera, Color dentro)
    {
        Cubo("LlamaFuera", padre, new Vector2(0f, 0.17f), new Vector2(0.5f, 0.46f), fuera, 4, triangulo);
        Cubo("LlamaDentro", padre, new Vector2(0f, 0.1f), new Vector2(0.28f, 0.28f), dentro, 5, triangulo);
    }

    // Circulo de luz (halo)
    static GameObject Halo(Transform padre, Color color, float diametro)
    {
        GameObject h = Cubo("Halo", padre, Vector2.zero, new Vector2(diametro, diametro), color, -6, circulo);
        return h;
    }

    static Sprite CrearCirculo()
    {
        string ruta = SPR + "/Circulo.png";
        int t = 128;
        Texture2D tex = new Texture2D(t, t, TextureFormat.RGBA32, false);
        for (int y = 0; y < t; y++)
            for (int x = 0; x < t; x++)
            {
                float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(t / 2f, t / 2f)) / (t / 2f);
                float a = Mathf.Clamp01(1f - d);
                a = Mathf.Pow(a, 0.6f);
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        File.WriteAllBytes(ruta, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
        AssetDatabase.ImportAsset(ruta);
        TextureImporter imp = (TextureImporter)AssetImporter.GetAtPath(ruta);
        imp.textureType = TextureImporterType.Sprite;
        imp.spriteImportMode = SpriteImportMode.Single;
        imp.spritePixelsPerUnit = t;
        imp.filterMode = FilterMode.Bilinear;
        imp.textureCompression = TextureImporterCompression.Uncompressed;
        imp.alphaIsTransparency = true;
        imp.mipmapEnabled = false;
        imp.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Sprite>(ruta);
    }

    static Transform Contenedor(Transform padre, string nombre)
    {
        GameObject g = new GameObject(nombre, typeof(RectTransform));
        g.transform.SetParent(padre, false);
        RectTransform rt = (RectTransform)g.transform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        return g.transform;
    }

    // Lumi dibujada con imagenes de UI (titulo y final)
    static void LumiUI(Transform padre, Vector2 pos, Color cuerpo, Color oscuro, Color ojos, int numOjos)
    {
        GameObject raiz = new GameObject("LumiDibujo", typeof(RectTransform));
        raiz.transform.SetParent(padre, false);
        ((RectTransform)raiz.transform).anchoredPosition = pos;
        Transform r = raiz.transform;
        Vector2 m = new Vector2(0.5f, 0.5f);
        Image llamaF = Imagen(r, "LlamaFuera", m, new Vector2(0, 118), new Vector2(100, 92), numOjos == 2 ? Hex("#FF5A1F") : Hex("#4A1F7A"));
        llamaF.sprite = triangulo;
        Image llamaD = Imagen(r, "LlamaDentro", m, new Vector2(0, 106), new Vector2(56, 56), numOjos == 2 ? AMARILLO : Hex("#B04BE0"));
        llamaD.sprite = triangulo;
        Imagen(r, "Cuerpo", m, new Vector2(0, 20), new Vector2(160, 140), cuerpo);
        Imagen(r, "Sombreado", m, new Vector2(0, -42), new Vector2(160, 16), oscuro);
        if (numOjos == 2) Imagen(r, "OjoIzq", m, new Vector2(6, 36), new Vector2(24, 40), ojos);
        Imagen(r, "OjoDer", m, new Vector2(50, 36), new Vector2(24, 40), ojos);
        Imagen(r, "Boca", m, new Vector2(28, -8), new Vector2(32, 10), numOjos == 2 ? NEGRO : ojos);
        Imagen(r, "PiernaIzq", m, new Vector2(-36, -70), new Vector2(32, 40), oscuro);
        Imagen(r, "PiernaDer", m, new Vector2(36, -70), new Vector2(32, 40), oscuro);
    }

    // ================= ANIMACIONES DE LUMI (y de Nadie y La Bruma) =================
    static AnimatorController ControladorLumi()
    {
        if (ctrlLumi != null) return ctrlLumi;
        const string T = "Visual/Torso", O = "Visual/Torso/Ojos", A = "Visual/Torso/Antena", PI = "Visual/PiernaIzq", PD = "Visual/PiernaDer";

        AnimationClip idle = Clip("Lumi_Idle", 2.4f, true);
        Pos(idle, T, new[] { 0f, 1.2f, 2.4f }, new[] { 0f, 0f, 0f }, new[] { 0.1f, 0.086f, 0.1f });
        Escala(idle, T, new[] { 0f, 1.2f, 2.4f }, new[] { 1f, 1.03f, 1f }, new[] { 1f, 0.96f, 1f });
        Escala(idle, O, new[] { 0f, 1.9f, 1.96f, 2.02f, 2.4f }, new[] { 1f, 1f, 1f, 1f, 1f }, new[] { 1f, 1f, 0.1f, 1f, 1f });
        Rot(idle, A, new[] { 0f, 0.6f, 1.2f, 1.8f, 2.4f }, new[] { 0f, 6f, 0f, -6f, 0f });

        float[] tc = { 0f, 0.1f, 0.2f, 0.3f, 0.4f };
        AnimationClip caminar = Clip("Lumi_Caminar", 0.4f, true);
        Pos(caminar, PI, tc, new[] { -0.26f, -0.18f, -0.10f, -0.18f, -0.26f }, new[] { -0.35f, -0.27f, -0.35f, -0.35f, -0.35f });
        Pos(caminar, PD, tc, new[] { 0.26f, 0.18f, 0.10f, 0.18f, 0.26f }, new[] { -0.35f, -0.35f, -0.35f, -0.27f, -0.35f });
        Pos(caminar, T, tc, new[] { 0f, 0f, 0f, 0f, 0f }, new[] { 0.1f, 0.14f, 0.1f, 0.14f, 0.1f });
        Rot(caminar, T, tc, new[] { -4f, -7f, -4f, -7f, -4f });
        Rot(caminar, A, tc, new[] { 14f, 8f, 14f, 8f, 14f });

        AnimationClip subir = Clip("Lumi_Subir", 0.3f, true);
        Escala(subir, T, new[] { 0f, 0.3f }, new[] { 0.86f, 0.86f }, new[] { 1.14f, 1.14f });
        Pos(subir, T, new[] { 0f, 0.3f }, new[] { 0f, 0f }, new[] { 0.14f, 0.14f });
        Pos(subir, PI, new[] { 0f, 0.3f }, new[] { -0.13f, -0.13f }, new[] { -0.31f, -0.31f });
        Pos(subir, PD, new[] { 0f, 0.3f }, new[] { 0.13f, 0.13f }, new[] { -0.31f, -0.31f });
        Escala(subir, O, new[] { 0f, 0.3f }, new[] { 1f, 1f }, new[] { 1.2f, 1.2f });
        Rot(subir, A, new[] { 0f, 0.15f, 0.3f }, new[] { -10f, -16f, -10f });

        AnimationClip caer = Clip("Lumi_Caer", 0.3f, true);
        Escala(caer, T, new[] { 0f, 0.3f }, new[] { 1.04f, 1.04f }, new[] { 0.96f, 0.96f });
        Pos(caer, PI, new[] { 0f, 0.15f, 0.3f }, new[] { -0.25f, -0.25f, -0.25f }, new[] { -0.40f, -0.35f, -0.40f });
        Pos(caer, PD, new[] { 0f, 0.15f, 0.3f }, new[] { 0.25f, 0.25f, 0.25f }, new[] { -0.35f, -0.40f, -0.35f });
        Escala(caer, O, new[] { 0f, 0.3f }, new[] { 1.1f, 1.1f }, new[] { 1.3f, 1.3f });
        Rot(caer, A, new[] { 0f, 0.15f, 0.3f }, new[] { 15f, 28f, 15f });

        AnimationClip aterrizar = Clip("Lumi_Aterrizar", 0.16f, false);
        Escala(aterrizar, T, new[] { 0f, 0.16f }, new[] { 1.25f, 1f }, new[] { 0.75f, 1f });
        Pos(aterrizar, T, new[] { 0f, 0.16f }, new[] { 0f, 0f }, new[] { 0.0125f, 0.1f });
        Pos(aterrizar, PI, new[] { 0f, 0.16f }, new[] { -0.25f, -0.18f }, new[] { -0.35f, -0.35f });
        Pos(aterrizar, PD, new[] { 0f, 0.16f }, new[] { 0.25f, 0.18f }, new[] { -0.35f, -0.35f });
        Rot(aterrizar, A, new[] { 0f, 0.16f }, new[] { -18f, 0f });

        AnimatorController ctrl = AnimatorController.CreateAnimatorControllerAtPath(ANI + "/Lumi.controller");
        ctrl.AddParameter("Velocidad", AnimatorControllerParameterType.Float);
        ctrl.AddParameter("VelY", AnimatorControllerParameterType.Float);
        ctrl.AddParameter("EnSuelo", AnimatorControllerParameterType.Bool);
        AnimatorStateMachine sm = ctrl.layers[0].stateMachine;
        AnimatorState sIdle = sm.AddState("Idle", new Vector3(300, 0)); sIdle.motion = idle;
        AnimatorState sCaminar = sm.AddState("Caminar", new Vector3(300, 120)); sCaminar.motion = caminar;
        AnimatorState sSubir = sm.AddState("Subir", new Vector3(600, -60)); sSubir.motion = subir;
        AnimatorState sCaer = sm.AddState("Caer", new Vector3(600, 60)); sCaer.motion = caer;
        AnimatorState sAterrizar = sm.AddState("Aterrizar", new Vector3(600, 180)); sAterrizar.motion = aterrizar;
        sm.defaultState = sIdle;

        Transicion(sIdle.AddTransition(sCaminar)).AddCondition(AnimatorConditionMode.Greater, 0.1f, "Velocidad");
        Transicion(sCaminar.AddTransition(sIdle)).AddCondition(AnimatorConditionMode.Less, 0.1f, "Velocidad");
        AnimatorStateTransition aSubir = Transicion(sm.AddAnyStateTransition(sSubir));
        aSubir.AddCondition(AnimatorConditionMode.IfNot, 0f, "EnSuelo");
        aSubir.AddCondition(AnimatorConditionMode.Greater, 0.5f, "VelY");
        aSubir.canTransitionToSelf = false;
        AnimatorStateTransition aCaer = Transicion(sm.AddAnyStateTransition(sCaer));
        aCaer.AddCondition(AnimatorConditionMode.IfNot, 0f, "EnSuelo");
        aCaer.AddCondition(AnimatorConditionMode.Less, -0.5f, "VelY");
        aCaer.canTransitionToSelf = false;
        Transicion(sCaer.AddTransition(sAterrizar)).AddCondition(AnimatorConditionMode.If, 0f, "EnSuelo");
        Transicion(sSubir.AddTransition(sAterrizar)).AddCondition(AnimatorConditionMode.If, 0f, "EnSuelo");
        AnimatorStateTransition finIdle = Transicion(sAterrizar.AddTransition(sIdle));
        finIdle.hasExitTime = true; finIdle.exitTime = 1f;
        finIdle.AddCondition(AnimatorConditionMode.Less, 0.1f, "Velocidad");
        AnimatorStateTransition finCaminar = Transicion(sAterrizar.AddTransition(sCaminar));
        finCaminar.hasExitTime = true; finCaminar.exitTime = 0.6f;
        finCaminar.AddCondition(AnimatorConditionMode.Greater, 0.1f, "Velocidad");

        ctrlLumi = ctrl;
        return ctrl;
    }

    // ================= AYUDANTES =================

    static Transform Vacio(string nombre, Transform padre, Vector2 pos)
    {
        Transform t = new GameObject(nombre).transform;
        t.SetParent(padre, false);
        t.localPosition = pos;
        return t;
    }

    static GameObject Cubo(string nombre, Transform padre, Vector2 pos, Vector2 escala, Color color, int orden, Sprite spr = null)
    {
        GameObject g = new GameObject(nombre);
        if (padre != null) g.transform.SetParent(padre, false);
        g.transform.localPosition = pos;
        g.transform.localScale = new Vector3(escala.x, escala.y, 1f);
        SpriteRenderer sr = g.AddComponent<SpriteRenderer>();
        sr.sprite = spr != null ? spr : cuadrado;
        sr.color = color;
        sr.sortingOrder = orden;
        if (matSprite != null) sr.sharedMaterial = matSprite;
        return g;
    }

    static GameObject Guardar(GameObject g, string nombre)
    {
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(g, PRE + "/" + nombre + ".prefab");
        Object.DestroyImmediate(g);
        return prefab;
    }

    static Sprite CrearSprite(string nombre, bool esTriangulo)
    {
        string ruta = SPR + "/" + nombre + ".png";
        int t = 16;
        Texture2D tex = new Texture2D(t, t, TextureFormat.RGBA32, false);
        for (int y = 0; y < t; y++)
        {
            for (int x = 0; x < t; x++)
            {
                bool lleno = true;
                if (esTriangulo)
                {
                    float mitadAncho = (1f - (y + 0.5f) / t) * t / 2f;
                    lleno = Mathf.Abs(x + 0.5f - t / 2f) <= mitadAncho + 0.25f;
                }
                tex.SetPixel(x, y, lleno ? Color.white : Color.clear);
            }
        }
        File.WriteAllBytes(ruta, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
        AssetDatabase.ImportAsset(ruta);

        TextureImporter imp = (TextureImporter)AssetImporter.GetAtPath(ruta);
        imp.textureType = TextureImporterType.Sprite;
        imp.spriteImportMode = SpriteImportMode.Single;
        imp.spritePixelsPerUnit = t;
        imp.filterMode = FilterMode.Point;
        imp.textureCompression = TextureImporterCompression.Uncompressed;
        imp.alphaIsTransparency = true;
        imp.mipmapEnabled = false;
        imp.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Sprite>(ruta);
    }

    static AnimationClip Clip(string nombre, float duracion, bool bucle)
    {
        string ruta = ANI + "/" + nombre + ".anim";
        AnimationClip existente = AssetDatabase.LoadAssetAtPath<AnimationClip>(ruta);
        if (existente != null) return existente;
        AnimationClip clip = new AnimationClip();
        clip.frameRate = 60;
        AssetDatabase.CreateAsset(clip, ruta);
        AnimationClipSettings ajustes = AnimationUtility.GetAnimationClipSettings(clip);
        ajustes.loopTime = bucle;
        ajustes.stopTime = duracion;
        AnimationUtility.SetAnimationClipSettings(clip, ajustes);
        return clip;
    }

    static AnimationCurve Curva(float[] t, float[] v)
    {
        Keyframe[] k = new Keyframe[t.Length];
        for (int i = 0; i < t.Length; i++) k[i] = new Keyframe(t[i], v[i]);
        AnimationCurve c = new AnimationCurve(k);
        for (int i = 0; i < k.Length; i++)
        {
            AnimationUtility.SetKeyLeftTangentMode(c, i, AnimationUtility.TangentMode.Linear);
            AnimationUtility.SetKeyRightTangentMode(c, i, AnimationUtility.TangentMode.Linear);
        }
        return c;
    }

    static float[] Constante(int n, float valor)
    {
        float[] a = new float[n];
        for (int i = 0; i < n; i++) a[i] = valor;
        return a;
    }

    static void Pos(AnimationClip clip, string ruta, float[] t, float[] x, float[] y)
    {
        clip.SetCurve(ruta, typeof(Transform), "m_LocalPosition.x", Curva(t, x));
        clip.SetCurve(ruta, typeof(Transform), "m_LocalPosition.y", Curva(t, y));
        clip.SetCurve(ruta, typeof(Transform), "m_LocalPosition.z", Curva(t, Constante(t.Length, 0f)));
        EditorUtility.SetDirty(clip);
    }

    static void Escala(AnimationClip clip, string ruta, float[] t, float[] x, float[] y)
    {
        clip.SetCurve(ruta, typeof(Transform), "m_LocalScale.x", Curva(t, x));
        clip.SetCurve(ruta, typeof(Transform), "m_LocalScale.y", Curva(t, y));
        clip.SetCurve(ruta, typeof(Transform), "m_LocalScale.z", Curva(t, Constante(t.Length, 1f)));
        EditorUtility.SetDirty(clip);
    }

    static void Rot(AnimationClip clip, string ruta, float[] t, float[] z)
    {
        clip.SetCurve(ruta, typeof(Transform), "localEulerAnglesRaw.x", Curva(t, Constante(t.Length, 0f)));
        clip.SetCurve(ruta, typeof(Transform), "localEulerAnglesRaw.y", Curva(t, Constante(t.Length, 0f)));
        clip.SetCurve(ruta, typeof(Transform), "localEulerAnglesRaw.z", Curva(t, z));
        EditorUtility.SetDirty(clip);
    }

    static AnimatorController ControladorSimple(string nombre, AnimationClip clip)
    {
        string ruta = ANI + "/" + nombre + ".controller";
        AnimatorController existente = AssetDatabase.LoadAssetAtPath<AnimatorController>(ruta);
        if (existente != null) return existente;
        AnimatorController ctrl = AnimatorController.CreateAnimatorControllerAtPath(ruta);
        AnimatorState s = ctrl.layers[0].stateMachine.AddState(clip.name);
        s.motion = clip;
        ctrl.layers[0].stateMachine.defaultState = s;
        return ctrl;
    }

    static AnimatorStateTransition Transicion(AnimatorStateTransition t)
    {
        t.hasExitTime = false;
        t.duration = 0f;
        return t;
    }

    static Text Texto(Transform padre, string nombre, string contenido, int tam, Color color, TextAnchor alineacion, Vector2 ancla, Vector2 pos, Vector2 tamano)
    {
        GameObject g = new GameObject(nombre, typeof(RectTransform), typeof(Text));
        g.transform.SetParent(padre, false);
        RectTransform rt = (RectTransform)g.transform;
        rt.anchorMin = ancla;
        rt.anchorMax = ancla;
        rt.pivot = ancla;
        rt.anchoredPosition = pos;
        rt.sizeDelta = tamano;
        Text t = g.GetComponent<Text>();
        t.font = fuente;
        t.text = contenido;
        t.fontSize = tam;
        t.color = color;
        t.alignment = alineacion;
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        t.raycastTarget = false;
        Outline borde = g.AddComponent<Outline>();
        borde.effectColor = NAVY;
        borde.effectDistance = new Vector2(3, -3);
        return t;
    }

    static Image Imagen(Transform padre, string nombre, Vector2 ancla, Vector2 pos, Vector2 tamano, Color color)
    {
        GameObject g = new GameObject(nombre, typeof(RectTransform), typeof(Image));
        g.transform.SetParent(padre, false);
        RectTransform rt = (RectTransform)g.transform;
        rt.anchorMin = ancla;
        rt.anchorMax = ancla;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = tamano;
        Image img = g.GetComponent<Image>();
        img.color = color;
        img.raycastTarget = false;
        return img;
    }
}
