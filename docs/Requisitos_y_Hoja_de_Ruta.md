# Wallpaper Manager

> Gestor de fondos de escritorio para Windows con biblioteca multimedia,
> perfiles independientes y administración de múltiples monitores.

**Estado del proyecto:** análisis y definición de requisitos\
**Versión del documento:** 1.0\
**Última actualización:** 14 de noviembre de 2026\
**Plataforma inicial:** Windows 11\
**Compatibilidad futura:** Windows 10, pendiente de validación\
**Implementación:** todavía no iniciada

------------------------------------------------------------------------

## 1. Descripción del proyecto

Wallpaper Manager es una aplicación de escritorio para Windows que
permite organizar, clasificar y reproducir fondos de pantalla estáticos
y animados. Está diseñada para gestionar varios monitores, perfiles de
contenido independientes, bibliotecas locales y externas, carruseles,
GIF y vídeos.

El objetivo es ofrecer una aplicación sencilla para usuarios no
técnicos, eficiente en recursos y suficientemente modular para
mantenerla, ampliarla y distribuirla públicamente.

El proyecto también se plantea como un proyecto de portfolio
profesional.

### Objetivos principales

-   Administrar una biblioteca centralizada de wallpapers.
-   Organizar el contenido mediante carpetas, etiquetas, favoritos y
    perfiles.
-   Gestionar cada monitor de forma independiente.
-   Reproducir imágenes, GIF y vídeos como fondo de escritorio.
-   Pausar o adaptar la reproducción según la actividad del equipo.
-   Continuar funcionando desde la bandeja del sistema.
-   Proteger los datos y recuperarse de errores.
-   Permitir personalizar la apariencia mediante temas JSON.
-   Distribuir la aplicación mediante un instalador `.exe`.

------------------------------------------------------------------------

## 2. Conceptos principales

  -----------------------------------------------------------------------
  Concepto                            Significado
  ----------------------------------- -----------------------------------
  Biblioteca                          Conjunto de todos los wallpapers
                                      registrados en la aplicación.

  Wallpaper                           Elemento multimedia registrado,
                                      identificado mediante un ID interno
                                      único.

  Ubicación                           Ruta física donde se encuentra un
                                      archivo. Un wallpaper puede tener
                                      varias ubicaciones si es duplicado.

  Carpeta                             Ubicación raíz registrada para
                                      explorar y sincronizar archivos.

  Etiqueta                            Clasificación personalizada
                                      asignada a uno o varios wallpapers.

  Favorito                            Clasificación especial personal,
                                      independiente de los perfiles.

  Perfil                              Selección y organización de
                                      wallpapers que puede asignarse a
                                      uno o varios monitores.

  Monitor                             Pantalla física detectada por
                                      Windows.

  Estado de reproducción              Configuración y posición de
                                      reproducción particular de cada
                                      monitor.
  -----------------------------------------------------------------------

**Distinción esencial:** el perfil contiene la selección y organización
del contenido. El estado de reproducción pertenece al monitor. Dos
monitores pueden utilizar el mismo perfil y reproducir elementos
distintos.

------------------------------------------------------------------------

## 3. Plataforma y formatos

### Plataforma

-   Primera plataforma objetivo: Windows 11.
-   La arquitectura debe evitar dependencias innecesarias de
    características exclusivas de Windows 11.
-   La compatibilidad con Windows 10 se evaluará mediante pruebas antes
    de anunciarla.
-   La distribución inicial será mediante instalador `.exe`.

### Formatos previstos

**Imágenes** - JPG / JPEG - PNG - WEBP - BMP

**Vectoriales** - SVG

**Animaciones** - GIF

**Vídeos** - MP4 - WEBM - MKV - MOV

La extensión de un archivo no garantiza que su contenido sea compatible.
La compatibilidad final dependerá del motor multimedia y de los códecs
disponibles. SVG puede requerir rasterización para utilizarse como
fondo.

------------------------------------------------------------------------

## 4. Requisitos funcionales

### 4.1. Biblioteca y carpetas

-   La biblioteca combinará una carpeta administrada por la aplicación y
    una o varias carpetas externas.
-   Los archivos externos no se moverán ni duplicarán al registrarlos.
-   La aplicación detectará archivos nuevos, modificados, eliminados,
    movidos o inaccesibles.
-   El escaneo será progresivo y se ejecutará en segundo plano.
-   El usuario podrá buscar y filtrar mientras continúa el escaneo.
-   El escaneo no debe bloquear la interfaz ni saturar innecesariamente
    CPU o disco.
-   Se guardarán los resultados para no analizar repetidamente archivos
    que no hayan cambiado.
-   Se deberá poder recuperar un escaneo interrumpido.
-   Se mostrará un indicador discreto de sincronización.
-   Los errores leves de sincronización irán al log; los graves seguirán
    el sistema general de notificaciones.

#### Duplicados

-   Si el mismo archivo aparece en varias carpetas, habrá una sola
    entrada lógica en la biblioteca.
-   Se conservarán todas las ubicaciones físicas conocidas.
-   La comparación de duplicados se basará en contenido, no solo en
    nombre.
-   Se recomienda comparar tamaño antes de calcular SHA-256.
-   Si desaparece una ubicación pero queda otra válida, el wallpaper
    debe seguir disponible.
-   La interfaz podrá ocultar visualmente los detalles de ubicación sin
    borrar esa información.

#### Eliminación

-   Si un archivo se elimina desde Explorer, la aplicación actualizará
    la biblioteca y los perfiles.
-   Si se elimina desde la aplicación, se advertirá si está incluido en
    perfiles.
-   El usuario deberá confirmar antes de eliminarlo de la biblioteca y
    de sus perfiles.
-   Borrar un archivo externo desde la aplicación elimina el archivo
    original; debe indicarse claramente.
-   Se utilizará la papelera de Windows, no una papelera propia.

### 4.2. Búsqueda y filtros

La búsqueda podrá utilizar:

-   Nombre del archivo.
-   Etiquetas.
-   Formato.
-   Resolución.
-   Favoritos.

No se requiere búsqueda semántica o reconocimiento del contenido visual.

Los filtros serán combinables. Inicialmente la búsqueda se ejecutará al
pulsar Enter, no en cada pulsación.

El buscador será compacto y flotante en la esquina inferior derecha. Al
activarlo, se desplegará hacia arriba mostrando búsqueda y filtros.

Si no hay coincidencias, se mostrarán alternativas ordenadas: 1.
Favoritos con mayor resolución. 2. Resto de favoritos. 3. No favoritos
con mayor resolución. 4. Resto de la biblioteca.

Los resultados se cargarán progresivamente, en bloques equivalentes a
seis filas, anticipando la carga al acercarse al final.

### 4.3. Tarjetas y dashboard

-   La pantalla principal tendrá navegación lateral y zona de contenido.
-   La zona principal mostrará un esquema dinámico de monitores y
    tarjetas de wallpapers.
-   El esquema deberá adaptarse a uno o varios monitores.
-   Seleccionar un monitor permitirá consultar su configuración.
-   Las tarjetas mostrarán una imagen grande y etiquetas.
-   No se mostrará el nombre del archivo como título principal de la
    tarjeta.
-   Al pasar el cursor, la tarjeta crecerá ligeramente sin solaparse con
    las demás.
-   Las acciones rápidas podrán incluir favoritos, asignación a perfiles
    e información.
-   Las acciones no dependerán exclusivamente del hover: también serán
    accesibles mediante clic y teclado.
-   La cuadrícula y el tamaño de miniaturas se adaptarán al espacio
    disponible.
-   Las preferencias visuales se conservarán por usuario.

Navegación prevista: - Dashboard - Biblioteca - Perfiles - Monitores -
Carpetas - Etiquetas - Configuración

La navegación definitiva se validará durante el diseño de interfaz.

### 4.4. Perfiles

-   Las listas de reproducción se denominan perfiles.
-   Un perfil contiene una selección y organización de wallpapers.
-   Un perfil puede asignarse a varios monitores.
-   Varios monitores pueden utilizar el mismo perfil.
-   Cada monitor mantiene su propio estado de reproducción.
-   Cambiar el contenido del perfil afecta a todos los monitores que lo
    utilizan.
-   Cambiar el intervalo o la posición de reproducción de un monitor no
    modifica los demás.
-   Los wallpapers añadidos a un perfil se colocan al final por defecto.
-   El orden dentro de un perfil es independiente del orden de la
    biblioteca.
-   Los perfiles guardan referencias a IDs de wallpapers, no copias de
    archivos.

### 4.5. Monitores

Cada monitor tendrá su propio: - Perfil asignado. - Intervalo de
cambio. - Orden secuencial o aleatorio. - Estado de pausa. - Posición
actual. - Estado de reproducción. - Preferencias de sincronización.

Al detectar un monitor nuevo: - Copiará temporalmente la configuración
del monitor principal. - Se podrá asignar otro perfil o modificar sus
opciones. - A partir de ese momento funcionará independientemente.

Al desconectar un monitor: - Se conservará su configuración y la fecha
de desconexión. - Se iniciará el periodo de espera configurado. - Al
llegar el momento correspondiente, se preguntará si se desea conservar
la configuración. - Si se cierra el aviso sin responder, se iniciará un
segundo periodo. - Si se responde que se desea conservarla, permanecerá
indefinidamente hasta su eliminación manual. - Si no se conserva ni se
reconecta dentro del segundo periodo, se eliminará automáticamente. -
Los temporizadores deberán sobrevivir al cierre de la aplicación.

### 4.6. Reproducción y automatización

La aplicación continuará funcionando en segundo plano al cerrar su
ventana principal.

Controles de bandeja: - Pausar/reanudar globalmente. - Silenciar/activar
sonido. - Siguiente fondo. - Fondo anterior. - Abrir aplicación. -
Salir.

El menú de bandeja actuará globalmente sobre todos los monitores. Los
controles individuales estarán en el dashboard.

Reglas generales: - Si se detecta un juego, se pausarán carruseles y
vídeos. - Si se detecta una aplicación a pantalla completa, se pausarán
los vídeos, pero los carruseles seguirán cambiando. - Al salir de la
situación que provoca la pausa, se recuperará el comportamiento
anterior. - Por defecto, el intervalo podrá provocar un cambio aunque el
vídeo no haya terminado. - Existirá una opción para esperar a que
termine el vídeo. - La detección utilizará capacidades nativas de
Windows, sin depender de Xbox Game Bar. - Se podrán definir excepciones
por aplicación: - Ignorar detección de juego. - Ignorar pantalla
completa. - Forzar pausa. - Predeterminado. - Las excepciones
prevalecerán sobre las reglas generales. - El ahorro de recursos será
automático y estará activo por defecto.

### 4.7. Usuarios y datos

Cada cuenta de Windows tendrá datos propios: - Perfiles. - Favoritos. -
Etiquetas. - Configuración. - Asignaciones de monitores. -
Preferencias. - Historial y estado.

La instalación podrá ser común para varios usuarios. Los datos
personales estarán separados.

Las carpetas externas podrán compartirse entre cuentas si Windows
permite el acceso. La clasificación personal nunca se compartirá
automáticamente.

La arquitectura no deberá depender conceptualmente del identificador de
Windows como único identificador permanente del usuario.

### 4.8. Importación y exportación

Se permitirá compartir perfiles entre equipos y usuarios.

La exportación incluirá: - Nombre y organización del perfil. -
Referencias de los wallpapers. - Miniaturas pequeñas para
identificarlos.

No incluirá: - Archivos originales. - Favoritos. - Etiquetas
personalizadas.

Al importar: - Se buscarán coincidencias en la biblioteca local. - Se
asociarán los wallpapers encontrados. - Se ignorarán los no
encontrados. - No se crearán placeholders ni referencias pendientes. -
No se duplicarán archivos. - Los IDs internos podrán ser diferentes
entre equipos.

Si el perfil ya existe, se preguntará si se desea sustituir. Si se crea
uno nuevo, se añadirá un sufijo `(2)`, `(3)`, etc.

### 4.9. Configuración

-   Una única página de configuración organizada en bloques temáticos.
-   Los bloques estarán abiertos por defecto.
-   Cada bloque podrá contraerse y su estado se recordará por usuario.
-   Las opciones menos habituales se agruparán en una sección Avanzado,
    contraída por defecto.
-   Los cambios se guardarán mediante un botón al final de la página.
-   Al salir con cambios pendientes se ofrecerá guardar, salir sin
    guardar o cancelar.
-   Habrá restablecimiento global y por bloque.
-   Restablecer no eliminará perfiles, favoritos, etiquetas, bibliotecas
    ni datos personales.
-   No habrá buscador de configuración en la primera versión.

### 4.10. Temas

-   Los temas serán archivos JSON situados en una carpeta `Themes`.
-   El tema inicial será Dark Purple.
-   Dark Purple estará protegido frente a eliminación desde la
    aplicación.
-   Los temas personalizados podrán añadirse, modificarse y eliminarse.
-   El gestor vigilará cambios en la carpeta.
-   Un tema inválido aparecerá identificado y no podrá aplicarse.
-   Si el tema activo desaparece o queda inválido, se aplicará Dark
    Purple.
-   Un tema defectuoso no impedirá iniciar la aplicación.
-   Los temas solo podrán modificar propiedades visuales; no ejecutarán
    código.
-   La selección se aplicará sin reiniciar.
-   Se mostrarán nombre y autor en el selector.
-   Los metadatos incluirán nombre, autor, ID, versión y descripción
    opcional.

### 4.11. Actualizaciones

-   Comprobar nuevas versiones como mínimo cada siete días.
-   La comprobación se realizará al abrir el Dashboard.
-   No descargar ni instalar sin acción del usuario.
-   Mostrar el aviso debajo de la navegación.
-   Permitir Actualizar o Descartar.
-   Descartar oculta el aviso, pero no elimina la actualización.
-   La actualización se podrá iniciar posteriormente desde
    Configuración.
-   Verificar integridad y autenticidad del paquete.
-   Mantener temporalmente la versión anterior.
-   Si falla la actualización, ejecutar rollback.
-   Preservar perfiles, favoritos, etiquetas, bibliotecas y
    configuraciones.
-   Contemplar los permisos de administrador cuando sean necesarios.

### 4.12. Backups y restauración

-   Backups opcionales.
-   Periodicidad configurable.
-   Una copia principal, sin acumulación de versiones.
-   Incluir perfiles, favoritos, etiquetas y configuraciones personales.
-   No incluir archivos multimedia originales.
-   No sustituir la copia anterior hasta validar la nueva.
-   Restauración manual desde Configuración.
-   Advertir que los datos actuales serán sustituidos.
-   Validar la copia antes de aplicarla.

### 4.13. Logs y notificaciones

Los logs registrarán: - Errores de carga. - Archivos movidos, eliminados
o inaccesibles. - Errores de reproducción. - Cambios de monitores. -
Sincronización. - Actualizaciones. - Importación y exportación. -
Errores internos.

Cada entrada incluirá fecha, hora, categoría, gravedad y detalles
técnicos.

Los logs serán locales y no se enviarán automáticamente por Internet.

Se utilizarán `app.log` y `app_backup.log` con rotación alterna,
conservando aproximadamente los dos últimos bloques.

No se desarrollará un visor de logs propio: se abrirán con un editor de
texto convencional.

Notificaciones: - Errores leves: solo log. - Errores graves: ventana con
descripción, parte afectada, estado y soluciones posibles. - Errores
críticos: impedirán el uso normal y mostrarán la ubicación de los
logs. - Los avisos de actualización permanecerán visibles hasta
gestionarse.

### 4.14. Integridad y recuperación

Al iniciar: - Comprobar archivos importantes de configuración. - Validar
estructura, tipos, valores y límites. - Detectar archivos dañados,
incompletos o incompatibles. - Registrar los errores. - Si los datos no
son válidos, avisar y ofrecer recuperación desde una copia anterior. -
No sobrescribir datos válidos sin permiso. - Si no existe copia, ofrecer
una recuperación alternativa segura.

Si un JSON principal presenta una estructura que incumple las reglas de
seguridad, la aplicación generará un error crítico e impedirá el inicio
normal.

Antes de guardar cualquier modificación en los JSON principales: -
Validar los datos. - Rechazar estructuras inválidas. - Conservar los
datos anteriores ante un fallo. - Registrar el error. - Informar al
usuario si no se puede completar la operación.

La validación debe centrarse en esquemas, campos, tipos, límites y
valores permitidos. Una modificación legítima no debe considerarse
maliciosa simplemente por ser diferente.

------------------------------------------------------------------------

## 5. Diseño visual y guía de estilos

### 5.1. Identidad visual inicial: Dark Purple

Paleta acordada:

  Uso                             Color
  ------------------------------- -----------
  Fondo principal                 `#0A090A`
  Fondo secundario / navegación   `#151316`
  Superficie elevada              `#201C22`
  Bordes y divisores              `#302936`
  Acento principal                `#C084FC`
  Hover de acento                 `#D8B4FE`
  Correcto                        `#4ADE80`
  Advertencia                     `#FBBF24`
  Error                           `#F87171`
  Información                     `#60A5FA`

Selección suave: `rgba(192, 132, 252, 0.12)`.

Tipografía sugerida: Inter. La disponibilidad y distribución de la
fuente deberá revisarse antes del empaquetado.

### 5.2. Principios de interfaz

-   Diseño oscuro, limpio y poco saturado.
-   Jerarquía visual clara.
-   Espaciado consistente.
-   Bordes discretos.
-   Acentos morados reservados para acciones y estados relevantes.
-   Evitar ventanas emergentes innecesarias.
-   Priorizar operaciones directas y comprensibles.
-   No ocultar acciones esenciales detrás de hover.
-   Mantener accesibilidad mediante teclado y estados visuales.

### 5.3. Layout

-   Navegación lateral.
-   Dashboard con monitores en la parte superior.
-   Biblioteca en cuadrícula.
-   Tarjetas con imagen predominante.
-   Etiquetas bajo cada tarjeta.
-   Acciones rápidas sobre la tarjeta.
-   Buscador flotante en esquina inferior derecha.
-   Configuración en bloques expandibles.

Las proporciones visuales iniciales del dashboard serán aproximadamente
1/4 para navegación y 3/4 para contenido, ajustables según resolución.

### 5.4. Temas personalizados

Los temas podrán cambiar: - Colores. - Fondos. - Superficies. -
Bordes. - Acentos. - Estados. - Selecciones.

No podrán cambiar: - Código. - Lógica. - Rutas internas. - Permisos. -
Comportamiento de reproducción.

------------------------------------------------------------------------

## 6. Requisitos no funcionales

### Rendimiento

-   La interfaz no se bloqueará durante escaneos ni operaciones largas.
-   Se utilizarán tareas asíncronas.
-   Se evitarán operaciones repetidas innecesarias.
-   Se liberarán recursos multimedia cuando ya no sean necesarios.
-   Se aplicará ahorro automático de recursos.
-   Las miniaturas se cargarán progresivamente.

### Estabilidad

-   La aplicación debe soportar sesiones prolongadas.
-   Debe recuperarse de interrupciones de escaneo.
-   Los fallos de un tema no afectarán al arranque.
-   Los fallos de actualización deben permitir rollback.
-   Las escrituras de datos deben ser seguras.

### Seguridad

-   Validación estricta de JSON.
-   Validación de rutas.
-   Respeto de permisos de Windows.
-   No ejecutar código desde temas ni archivos importados.
-   Verificación de paquetes de actualización.
-   Separación de datos por usuario.

### Mantenibilidad

-   Arquitectura modular.
-   Separación de interfaz, dominio e infraestructura.
-   Contratos claros entre módulos.
-   Versionado de esquemas.
-   Pruebas automatizadas.
-   Documentación de decisiones técnicas.

------------------------------------------------------------------------

## 7. Arquitectura técnica propuesta (pendiente de aprobación)

Se recomienda inicialmente un **monolito modular**: un ejecutable
principal dividido internamente en módulos con responsabilidades
independientes. No se crearán procesos separados sin una justificación
técnica.

Módulos previstos:

1.  Presentación.
2.  Aplicación y coordinación.
3.  Dominio y reglas.
4.  Biblioteca.
5.  Escáner y sincronización.
6.  Perfiles.
7.  Monitores.
8.  Reproducción multimedia.
9.  Automatización.
10. Configuración.
11. Temas.
12. Integración con Windows.
13. Logs y recuperación.
14. Actualizaciones.

Los módulos se comunicarán mediante interfaces y eventos internos.

La posibilidad de aislar el reproductor en un proceso separado se
evaluará mediante prototipos, especialmente por la estabilidad de
bibliotecas multimedia nativas.

------------------------------------------------------------------------

## 8. Tecnologías candidatas (ninguna aprobada todavía)

  -----------------------------------------------------------------------
  Área                    Candidatos              Criterio de elección
  ----------------------- ----------------------- -----------------------
  Lenguaje                C#                      Integración Windows,
                                                  ecosistema y
                                                  mantenibilidad.

  Runtime                 .NET 10 LTS             Estabilidad y soporte a
                                                  largo plazo.

  Interfaz                WPF / WinUI 3           Diseño, rendimiento,
                                                  despliegue y
                                                  compatibilidad.

  Multimedia              Media Foundation /      Formatos, aceleración,
                          LibVLC                  estabilidad y licencia.

  Procesamiento auxiliar  FFmpeg                  Capacidades y
                                                  obligaciones de
                                                  licencia.

  Configuración           JSON                    Legibilidad y facilidad
                                                  de edición controlada.

  Catálogo                SQLite                  Consultas, relaciones e
                                                  integridad.

  Escaneo                 APIs .NET +             Rendimiento y
                          FileSystemWatcher       reconciliación.

  Hash                    SHA-256                 Identificación de
                                                  contenido.

  Monitores               APIs nativas Windows    Detección y
                                                  configuración.

  Instalación             EXE convencional / MSIX Compatibilidad y
                                                  experiencia de
                                                  instalación.
  -----------------------------------------------------------------------

### Decisiones técnicas pendientes

-   Framework de interfaz definitivo.
-   Motor multimedia.
-   Técnica para alojar fondos animados detrás de los iconos del
    escritorio.
-   JSON o SQLite para el catálogo.
-   Modelo de sincronización entre monitores.
-   Aislamiento del reproductor.
-   Gestión de códecs y licencias.
-   Política de caché.
-   Soporte de HDR y DPI.
-   Firma digital y actualización.
-   Instalación por usuario o para todos.
-   Compatibilidad y límites de Windows 10.

No se debe empezar la implementación completa hasta resolver las
decisiones críticas y aprobar el diseño técnico.

------------------------------------------------------------------------

## 9. Riesgos técnicos conocidos

  -----------------------------------------------------------------------
  Riesgo                              Medida prevista
  ----------------------------------- -----------------------------------
  Fondos animados detrás de iconos    Prototipo temprano de integración
                                      con el escritorio.

  Consumo de varios vídeos            Pruebas de CPU, RAM y GPU; límites
                                      de reproducción.

  Diferencias entre monitores         Pruebas de resolución, DPI,
                                      orientación y HDR.

  Eventos perdidos del sistema de     Escaneo de reconciliación.
  archivos                            

  JSON corrupto                       Validación, backup y recuperación
                                      aislada.

  Actualización fallida               Verificación y rollback.

  Licencias multimedia                Auditoría antes de distribuir.

  Bibliotecas grandes                 Caché, carga progresiva y consultas
                                      eficientes.

  Detección incorrecta de juegos      Excepciones configurables.

  Permisos insuficientes              Separación entre instalación y
                                      datos de usuario.
  -----------------------------------------------------------------------

------------------------------------------------------------------------

## 10. Decisiones funcionales todavía por concretar

1.  Si una ubicación desaparece pero queda otra del mismo wallpaper, se
    mantendrá la entrada lógica.
2.  Qué sucede si vence el intervalo mientras un vídeo está pausado.
3.  Qué aspectos exactos se sincronizan entre monitores: contenido,
    posición, temporizador y orden aleatorio.
4.  Si descartar cambios pendientes revierte también los efectos
    visuales aplicados inmediatamente.
5.  Cómo se comportan las carpetas de red, unidades extraíbles y enlaces
    simbólicos.
6.  Volumen inicial y sonido predeterminado de los vídeos.
7.  Qué sucede si una actualización necesita permisos de administrador y
    el usuario no los tiene.
8.  Qué datos se conservan al desinstalar.
9.  Qué límites tendrá la personalización de temas.
10. Qué ocurre cuando un archivo de una biblioteca compartida se elimina
    desde otra cuenta.

Estas cuestiones deben resolverse durante el cierre del análisis
funcional.

------------------------------------------------------------------------

## 11. Hoja de ruta

### Fase 0 --- Cierre del análisis

-   Completar requisitos.
-   Resolver contradicciones.
-   Aprobar criterios de aceptación.
-   Elaborar registro de decisiones.

**Hito:** requisitos aprobados.

### Fase 1 --- Prototipos de viabilidad

-   Ventanas de fondo animado.
-   Varios monitores.
-   Reproducción multimedia.
-   Pausa y reanudación.
-   Bandeja del sistema.
-   Detección de pantalla completa.
-   Pruebas de consumo.

**Hito:** demostrar que las funciones más arriesgadas son viables.

### Fase 2 --- Selección tecnológica

-   Comparar WPF y WinUI 3.
-   Comparar motores multimedia.
-   Evaluar JSON y SQLite.
-   Auditar licencias.
-   Definir arquitectura.

**Hito:** diseño técnico aprobado.

### Fase 3 --- Núcleo

-   Usuarios.
-   Configuración.
-   Persistencia.
-   Validación.
-   Logs.
-   Backups.
-   Recuperación.

**Hito:** datos seguros y recuperables.

### Fase 4 --- Biblioteca

-   Escaneo.
-   Carpetas.
-   Duplicados.
-   Miniaturas.
-   Etiquetas.
-   Favoritos.
-   Búsqueda.

**Hito:** biblioteca funcional y eficiente.

### Fase 5 --- Perfiles y monitores

-   Perfiles.
-   Asignaciones.
-   Estados independientes.
-   Sincronización.
-   Desconexión y reconexión.

**Hito:** varios monitores funcionando correctamente.

### Fase 6 --- Reproducción y automatización

-   Imágenes.
-   GIF.
-   Vídeos.
-   Carruseles.
-   Sonido.
-   Reglas automáticas.
-   Ahorro de recursos.

**Hito:** reproducción estable en sesiones prolongadas.

### Fase 7 --- Experiencia de usuario

-   Interfaz final.
-   Temas.
-   Configuración.
-   Importación y exportación.
-   Notificaciones.
-   Accesibilidad.

**Hito:** funcionalidades completas.

### Fase 8 --- Distribución

-   Instalador.
-   Firma.
-   Actualizaciones.
-   Rollback.
-   Desinstalación.

**Hito:** instalación y actualización verificadas.

### Fase 9 --- Pruebas y publicación

-   Rendimiento.
-   Seguridad.
-   Compatibilidad.
-   Pruebas de larga duración.
-   Documentación.
-   Preparación de versión pública.

**Hito:** versión estable lista para distribuir.

------------------------------------------------------------------------

## 12. Documentación oficial del proyecto

El proyecto mantendrá tres documentos:

### A. Requisitos

Este documento Markdown, ampliado y versionado conforme se aprueben
decisiones.

### B. Diseño técnico

Deberá recoger: - Arquitectura final. - Diagramas. - Módulos. -
Interfaces internas. - Modelo de datos. - Tecnologías aprobadas. -
Dependencias. - Seguridad. - Instalación y actualización.

### C. Plan de desarrollo y pruebas

Deberá recoger: - Fases. - Tareas. - Prioridades. - Dependencias. -
Hitos. - Pruebas unitarias. - Pruebas de integración. - Criterios de
aceptación.

------------------------------------------------------------------------

## 13. Condición para comenzar el desarrollo

La implementación completa comenzará cuando:

-   Los requisitos críticos estén cerrados.
-   Se haya demostrado la viabilidad de los fondos animados.
-   Se haya elegido el framework de interfaz.
-   Se haya elegido el motor multimedia.
-   Se haya aprobado el almacenamiento.
-   Se haya definido la recuperación de datos.
-   Se haya aprobado la arquitectura.
-   Exista un plan de pruebas.

**Principio del proyecto:** priorizar estabilidad, claridad, eficiencia
y facilidad de mantenimiento frente a añadir funcionalidades
rápidamente.

------------------------------------------------------------------------

## 14. Registro de cambios

  -----------------------------------------------------------------------
  Versión                 Fecha                   Cambios
  ----------------------- ----------------------- -----------------------
  1.0                     2026-11-14              Primera consolidación
                                                  de visión, requisitos,
                                                  arquitectura candidata
                                                  y hoja de ruta.

  -----------------------------------------------------------------------
