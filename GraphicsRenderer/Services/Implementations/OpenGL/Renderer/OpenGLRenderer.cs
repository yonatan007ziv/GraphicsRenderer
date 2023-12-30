﻿using GraphicsRenderer.Components.Extensions;
using GraphicsRenderer.Components.Interfaces;
using GraphicsRenderer.Components.OpenGL;
using GraphicsRenderer.Components.Shared;
using GraphicsRenderer.Services.Interfaces.InputProviders;
using GraphicsRenderer.Services.Interfaces.Renderer;
using GraphicsRenderer.Services.Interfaces.Utils;
using GraphicsRenderer.Services.Interfaces.Utils.Managers;
using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL4;
using System.Numerics;

namespace GraphicsRenderer.Services.Implementations.OpenGL.Renderer;

internal class OpenGLRenderer : BaseOpenGLRenderer, IRenderer
{
	public static OpenGLRenderer Instance; // TEMP!

	private readonly IInputProvider inputProvider;
	private readonly ILogger logger;
	private readonly IGameObjectManager gameObjectManager;
	private readonly IShaderManager shaderManager;
	private readonly IModelImporter importer;
	private readonly ISceneManager sceneManager;

	// Temps
	public ICamera camera;
	private MovementController movementController;
	private GameObject trex;
	private GameObject plane;

	public OpenGLRenderer(IInputProvider inputProvider, ILogger logger, IGameObjectManager gameObjectManager, IShaderManager shaderManager, ISettingsManager settingsManager, IModelImporter importer, ISceneManager sceneManager)
		: base(settingsManager)
	{
		Instance = this;

		this.inputProvider = inputProvider;
		this.logger = logger;
		this.gameObjectManager = gameObjectManager;
		this.shaderManager = shaderManager;
		this.importer = importer;
		this.sceneManager = sceneManager;

		LockMouse(true);
	}

	protected override void RenderFrame(float deltaTime)
	{
		// sceneManager.CurrentScene.RenderScene(deltaTime);

		foreach (GameObject gameObject in gameObjectManager.GameObjects)
			gameObject.Render(camera);
	}

	protected override void UpdateFrame(float deltaTime)
	{
		// sceneManager.CurrentScene.UpdateScene(deltaTime);


		// trex.Transform.Position = new System.Numerics.Vector3((float)MathHelper.Cos(GLFW.GetTime()), 0, (float)MathHelper.Sin(GLFW.GetTime())) * 25;
		movementController.UpdateInput(inputProvider, deltaTime);
		camera.Update(MouseState.Position.ToNumerics(), deltaTime);
	}

	protected override void Load()
	{
		logger.LogInformation("Loading Renderer...");

		shaderManager.RegisterShaders();
		// sceneManager.LoadScene("MainScene.scene");

		GameObject player = gameObjectManager.CreateGameObject();
		player.Transform.Position = new Vector3(0, 100, 0);
		camera = new OpenGLCamera(player, 10, Width, Height);
		movementController = new MovementController(player);

		trex = gameObjectManager.CreateGameObject();
		trex.Mesh = new OpenGLMesh(importer.ImportModel("TREX.obj"));
		trex.Shader = shaderManager.GetShader("Textured");

		plane = gameObjectManager.CreateGameObject();
		plane.Mesh = new OpenGLMesh(importer.ImportModel("Plane.obj"));
		plane.Shader = shaderManager.GetShader("Textured");
		plane.Transform.Scale = new Vector3(100, 0, 100);
	}

	protected override void Unload()
	{
		shaderManager.DisposeAll();
	}

	protected override void Resize(int width, int height)
	{
		if (camera != null)
		{
			camera.Width = width;
			camera.Height = height;
		}
	}

	protected override void GLDebugCallback(string msg, DebugSeverity severity)
	{
		switch (severity)
		{
			case DebugSeverity.DebugSeverityLow:
				logger.LogInformation("GL-INFO: {errorMsg}", msg);
				break;
			case DebugSeverity.DebugSeverityMedium:
				logger.LogError("GL-ERROR: {errorMsg}", msg);
				break;
			case DebugSeverity.DebugSeverityHigh:
				logger.LogCritical("GL-CRITICAL: {errorMsg}", msg);
				break;
			case DebugSeverity.DebugSeverityNotification:
				logger.LogTrace("GL-TRACE: {errorMsg}", msg);
				break;
		}
	}
}