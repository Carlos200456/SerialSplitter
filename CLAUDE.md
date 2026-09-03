# SerialSplitter

App WinForms (.NET Framework 4.8) que hace de splitter serie para equipos de rayos.
Solución de Visual Studio con proyectos clásicos (no SDK-style).

## Dependencia de otro repo — leer antes de compilar

`SerialSplitter.sln` incluye dos proyectos que viven en un **repo git distinto**,
`VISUB_Control`, como carpeta hermana:

```
source/repos/Carlos200456/
├── SerialSplitter/      <- este repo
└── VISUB_Control/       <- repo separado (rama Multi_KV)
    ├── SimpleUDP/       <- ProjectReference real de SerialSplitter
    └── TestUDP/         <- solo está en la solución, NO es dependencia
```

Son dos repos separados, no un monorepo. **Hay que clonar/pullear los dos.**
Si `VISUB_Control` no está al día, SerialSplitter no compila en ninguna rama:
falla la build de la dependencia y eso además rompe el designer de WinForms.

Al pushear cambios que toquen ambos, mandar **`VISUB_Control` primero**.

## Plataforma: solo x64

Todo va en x64 por compatibilidad con DRA.XA x64 (equipos con PC i9).
No se usa AnyCPU ni x86: esas configuraciones fueron eliminadas a propósito
de los `.csproj` y del `.sln`.

Configuraciones válidas: **`Debug|x64`** y **`Release|x64`**, y nada más.
`SimpleUDP` y `TestUDP` tienen sus propias configs x64 y se mapean a x64 real.

No volver a agregar AnyCPU/x86 ni `Prefer32Bit` (solo aplica a AnyCPU; en un
grupo x64 el compilador lo ignora y confunde).

## Compilar

```sh
MSBuild.exe SerialSplitter.sln /t:Build /p:Configuration=Debug /p:Platform=x64
```

MSBuild suele estar en:
`C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe`

Compilar el `.sln`, no el `.csproj` suelto con `/p:Platform=x64`: como propiedad
global, ese platform se propaga a las dependencias y saltea el mapeo de la solución.

Verificar que el binario salió x64 de verdad: header PE debe decir AMD64 / PE32+.

## Ramas

Cada rama produce un ejecutable distinto (ojo, cambia el `AssemblyName`):

| Rama | AssemblyName |
|---|---|
| `master` | `SerialSplitter` |
| `CAN-Colimator` | `SerialSplitterArcoCAN` |
| `Arco_Colimador` | `SerialSplitterArco` |

## El designer de WinForms no abre

Síntoma típico, y engañoso:

> The base class 'System.Windows.Forms.Form' could not be loaded.
> Ensure the assembly has been referenced and that all projects have been built.

Casi nunca es un problema del código del form. Revisar en este orden:

1. ¿Compila la solución en `Debug|x64`? Si falla una dependencia, el designer no carga.
2. ¿Está `VISUB_Control` al día?
3. ¿Se cambió de rama con VS abierto? Ver abajo.

Si VS quedó en mal estado: cerrarlo, borrar `.vs`, `bin` y `obj`, reabrir y Rebuild.

## No cambiar de rama con Visual Studio abierto

Como el `AssemblyName` y el `.sln` cambian entre ramas, un `git checkout` externo
deja a VS con el modelo de proyecto de la rama anterior, buscando un ensamblado
que ya no existe. Resultado: el designer deja de abrir con el error de arriba.

Hacer el cambio de rama desde VS, o cerrar VS antes.

Para pushear una rama que no está activa no hace falta checkout:

```sh
git push origin RAMA:RAMA
```

Para editar archivos de otra rama sin tocar el working tree, usar `git worktree`.
