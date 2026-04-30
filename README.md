# Lamento-del-Guardian





\# 🛡️ Políticas de Desarrollo - Lamento del Guardian



Bienvenido al repositorio central de \*\*Lamento del Guardian\*\*. 

Dado que somos un equipo de 5 personas y todos estaremos colaborando en distintas áreas del juego (programación en C#, diseño de niveles, modelos 3D, etc.) al mismo tiempo, es \*\*obligatorio\*\* seguir estas reglas para evitar que el proyecto se corrompa o perdamos el trabajo de algún compañero.



\---



\## 🚦 Regla 0: La Rutina Diaria (¡Obligatoria!)

Cada vez que te sientes a trabajar en tu computadora, debes seguir este orden exacto:

1\. \*\*Abre GitHub Desktop ANTES que Unity.\*\*

2\. Haz clic en \*\*Fetch origin\*\* y luego en \*\*Pull\*\* para descargar los últimos cambios del equipo.

3\. Abre Unity y comprueba que el proyecto funciona correctamente.

4\. Al terminar tu sesión de trabajo, guarda en Unity, ve a GitHub Desktop, haz un \*\*Commit\*\* y presiona \*\*Push\*\*.



\## 🌿 Reglas de Ramas (Branching)

\*\*ESTÁ PROHIBIDO TRABAJAR DIRECTAMENTE EN `main` O `develop`.\*\*

\* \*\*`main`\*\*: Es la versión final del juego. Solo se actualiza cuando tenemos una versión estable y jugable.

\* \*\*`develop`\*\*: Es nuestra rama de integración. Aquí juntamos el trabajo de todos para probarlo.

\* \*\*Tu rama de trabajo\*\*: Cada vez que vayas a hacer algo, crea una rama nueva a partir de `develop`.

&#x20; \* \*Nomenclatura:\* Usa el prefijo `feature/` seguido de tu tarea. 

&#x20; \* \*Ejemplo:\* `feature/movimiento-jugador` o `feature/texturas-nivel1`.



\## 🎮 Reglas para Unity (Para evitar el apocalipsis de Git)

Unity y Git pueden llevarse mal si no somos cuidadosos con los archivos binarios y las escenas.

1\. \*\*Todo es un Prefab:\*\* No construyas objetos complejos directamente en la escena. Crea tu personaje, enemigo o menú dentro de un \*\*Prefab\*\*. Así, varios pueden trabajar en distintos prefabs simultáneamente sin causar conflictos.

2\. \*\*Cuidado con las Escenas:\*\* Si dos personas editan y guardan la misma escena (ej. `Nivel\_Principal.unity`) al mismo tiempo, el archivo se corromperá. \*\*Pregunta siempre\*\* por el grupo de chat antes de modificar una escena principal.

3\. \*\*No subas basura:\*\* Si ves que GitHub Desktop intenta subir archivos de las carpetas `Library/`, `Temp/`, `Logs/` o archivos inusualmente pesados (+50MB), \*\*cancela el commit y avisa al equipo\*\*. 



\## 📝 Reglas de Commits y Pull Requests

\* \*\*Commits claros:\*\* No pongas mensajes como \*"asdfg"\* o \*"cosas"\*. Explica qué hiciste para que el resto del equipo entienda. 

&#x20; \* \*Bien:\* "Agregado script de daño al jugador" / \*Mal:\* "cambios xd"

\* \*\*Pull Requests (PR):\*\* Cuando termines tu rama `feature/...`, publícala en GitHub y abre un Pull Request hacia `develop`. 

\* \*\*Revisión:\*\* Trata de que al menos un compañero revise tu PR y pruebe que el juego compila antes de darle a \*Merge\*.



\## 📢 Comunicación

La herramienta más poderosa que tenemos no es Git, es hablar entre nosotros. Si vas a modificar un script base de C# o una escena importante, avisa primero por nuestro grupo de comunicación.

