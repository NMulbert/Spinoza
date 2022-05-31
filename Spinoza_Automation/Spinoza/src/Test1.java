import java.util.Date;
import java.util.concurrent.TimeUnit;
import org.openqa.selenium.By;
import org.openqa.selenium.Keys;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.chrome.ChromeDriver;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;
import org.testng.Assert;

public class Test1 {

	public static void main(String[] args) throws InterruptedException {
		// Invoke Browser
		// Tests Create Test

		System.setProperty("webdriver.chrome.driver", ".\\lib\\chromedriver.exe");
		WebDriver driver = new ChromeDriver();
		driver.manage().timeouts().implicitlyWait(15, TimeUnit.SECONDS);

		driver.manage().window().maximize();
//		driver.get("http://localhost:3000/");
		driver.get("https://www.zionetapp.com/");

		
		// TEST'S CREATE TEST
		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
		System.out.println("TEST START - TEST'S CREATE");
		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");

		System.out.println("TEST START - TEST 1");
		CreateNewTest(driver);  // CREATE TEST AND CHECK TITLE CONFLICT
		System.out.println("TEST END - TEST 1");

		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
		Thread.sleep(5000);

		System.out.println("TEST START - TEST 2");
		CreateNewTest_Test1(driver); // CREATE TEST WITHOUT DATA
		System.out.println("TEST END - TEST 2");

		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
		Thread.sleep(5000);

		System.out.println("TEST START - TEST 3");
		CreateNewTest_Test2(driver); // CREATE TEST WITH TITLE ONLY
		System.out.println("TEST END - TEST 3");

		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
		Thread.sleep(5000);

		System.out.println("TEST START - TEST 4");
		CreateNewTest_Test3(driver); // CREATE TEST WITH ALL DATA FIELDS
		System.out.println("TEST END - TEST 4");

		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
		System.out.println("TEST END - TEST'S CREATE");
		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");

		Thread.sleep(5000);
		driver.quit();
	}

	public static void CreateNewTest(WebDriver driver) {
		try {
			// CREATE TEST AND CHECK TITLE CONFLICT
			driver.findElement(By.xpath("//span[contains(text(),'Tests')]")).click();
			Thread.sleep(5000);
			driver.findElement(By.xpath("//span[contains(text(),'NEW TEST')]")).click();

			AddTitle(driver, "Test Conflict");

			driver.findElement(By.cssSelector(
					"button[class='mantine-Button-gradient mantine-Button-root mantine-Group-child mantine-e8vnjr']"))
					.click();

			WebDriverWait wait = new WebDriverWait(driver, 15);
			wait.until(ExpectedConditions.visibilityOfElementLocated(By.xpath("//div[@role='alert']")));

			String alert = driver.findElement(By.xpath("//div[@role='alert']")).getText();

			if (!alert.contains("Conflict")) {
				driver.findElement(By.cssSelector(
						"button[class='mantine-Button-gradient mantine-Button-root mantine-Group-child mantine-e8vnjr']"))
						.click();
				CreateTest3_Check(driver);

			} else {
				System.out.println("Create Test = SUCCESS");

			}

			driver.findElement(By.xpath("//span[contains(text(),'Tests')]")).click();

		} catch (Exception e) {
			System.out.println("Error = CreateNewTest");
			System.out.println(e);
		}

	}

	public static void CreateNewTest_Test1(WebDriver driver) {
		try {
			// CREATE TEST WITHOUT DATA
			driver.findElement(By.xpath("//span[contains(text(),'Tests')]")).click();
			Thread.sleep(5000);
			driver.findElement(By.xpath("//span[contains(text(),'NEW TEST')]")).click();

			driver.findElement(By.cssSelector(
					"button[class='mantine-Button-gradient mantine-Button-root mantine-Group-child mantine-e8vnjr']"))
					.click();

			CreateTest1_Check(driver);

			Thread.sleep(3000);
			
			driver.findElement(By.xpath("//span[contains(text(),'Tests')]")).click();

		} catch (Exception e) {
			System.out.println("Error = CreateNewTest_Test1");
			System.out.println(e);
		}
	}

	public static void CreateNewTest_Test2(WebDriver driver) {
		try {
			// CREATE TEST WITH TITLE ONLY
			driver.findElement(By.xpath("//span[contains(text(),'Tests')]")).click();
			Thread.sleep(5000);
			driver.findElement(By.xpath("//span[contains(text(),'NEW TEST')]")).click();

			AddTitle(driver);

			driver.findElement(By.cssSelector(
					"button[class='mantine-Button-gradient mantine-Button-root mantine-Group-child mantine-e8vnjr']"))
					.click();

			WebDriverWait wait = new WebDriverWait(driver, 15);
			wait.until(ExpectedConditions.visibilityOfElementLocated(By.xpath("//div[@role='alert']")));

			CreateTest2_Check(driver);

			driver.findElement(By.xpath("//span[contains(text(),'Tests')]")).click();

		} catch (Exception e) {
			System.out.println("Error = CreateNewTest_Test2");
			System.out.println(e);
		}
	}

	public static void CreateNewTest_Test3(WebDriver driver) {
		try {
			// CREATE TEST WITH ALL DATA FIELDS
			driver.findElement(By.xpath("//span[contains(text(),'Tests')]")).click();
			Thread.sleep(5000);
			driver.findElement(By.xpath("//span[contains(text(),'NEW TEST')]")).click();

			// TITLE
			AddTitle(driver);

			// DESCRIPTION
			AddDescription(driver);

			// TAGS
			AddTags(driver);

			// SAVE
			SaveTest(driver);

		} catch (Exception e) {
			System.out.println("Error = CreateNewTest_Test3");
			System.out.println(e);
		}
	}

	private static void AddTitle(WebDriver driver) {
		try {
			Date date = new Date();
			String s = date.toLocaleString();
			WebElement TestTitle = driver.findElement(By.xpath("//input[@placeholder='Test title']"));
			TestTitle.clear();
			TestTitle.sendKeys("Automation TEST | " + s);
			System.out.println("TITLE = 'NEW TEST' | " + s);

		} catch (Exception e) {
			System.out.println("Error = Test Title");
			System.out.println(e);
		}
	}

	private static void AddTitle(WebDriver driver, String title) {
		try {
			WebElement TestTitle = driver.findElement(By.xpath("//input[@placeholder='Test title']"));
			TestTitle.clear();
			TestTitle.sendKeys("Automation TEST | " + title);
			System.out.println("TITLE = 'NEW TEST' | " + title);

		} catch (Exception e) {
			System.out.println("Error = Test Title");
			System.out.println(e);
		}
	}

	private static void AddDescription(WebDriver driver) {
		try {
			WebElement Description = driver.findElement(By.cssSelector(".w-md-editor-text-input"));
			Description.click();
			Description.clear();
			Description.sendKeys("TEST DESCRIPTION");
			System.out.println("DESCRIPTION = 'TEST DESCRIPTION'");

		} catch (Exception e) {
			System.out.println("Error = Description");
			System.out.println(e);
		}
	}

	private static void AddTags(WebDriver driver) {
		try {
			WebElement Tags = driver.findElement(By.xpath("//input[@placeholder='#Tags']"));
			Tags.sendKeys("React", Keys.ENTER);
			Tags.sendKeys("C#", Keys.ENTER);
			Tags.sendKeys("JavaScript", Keys.ENTER);
			Tags.sendKeys("Python", Keys.ENTER);
			Tags.click();
			System.out.println("TAGS = ['React','C#','JavaScript','Python']");

		} catch (Exception e) {
			System.out.println("Error = Tags");
			System.out.println(e);
		}
	}

	private static void SaveTest(WebDriver driver) {
		try {
			driver.findElement(By.cssSelector(
					"button[class='mantine-Button-gradient mantine-Button-root mantine-Group-child mantine-e8vnjr']"))
					.click();

			WebDriverWait wait = new WebDriverWait(driver, 15);
			wait.until(ExpectedConditions.visibilityOfElementLocated(By.xpath("//div[@role='alert']")));

			String alert = driver.findElement(By.xpath("//div[@role='alert']")).getText();

			if (alert.contains("InternalServerError")) {
				System.out.println("Create Test = FAILED");
			} else {
				System.out.println("Create Test = SUCCESS");
			}

			driver.findElement(By.xpath("//span[contains(text(),'Tests')]")).click();

		} catch (Exception e) {
			System.out.println("Error = Save As Draft");
			System.out.println(e);
		}
	}

	private static void CreateTest1_Check(WebDriver driver) {
		try {
			Assert.assertTrue(
					driver.findElement(By.cssSelector(".mantine-Text-root.mantine-TextInput-error.mantine-sy7825"))
							.getText().contains("required field"));
			System.out.println("Create Test1 = SUCCESS");
		} catch (Exception e) {
			System.out.println("Create Test1 = FAILED");
			System.out.println(e);
		}
	}

	private static void CreateTest2_Check(WebDriver driver) {
		try {
			Assert.assertTrue(driver.findElement(By.xpath("//div[@role='alert']")).getText().contains("created"));
			System.out.println("Create Test2 = SUCCESS");
		} catch (Exception e) {
			System.out.println("Create Test2 = FAILED");
			System.out.println(e);
		}
	}

	private static void CreateTest3_Check(WebDriver driver) {
		try {
			Assert.assertTrue(driver.findElement(By.xpath("//div[@role='alert']")).getText().contains("Conflict"));
			System.out.println("Create Test3 = SUCCESS");
		} catch (Exception e) {
			System.out.println("Create Test3 = FAILED");
			System.out.println(e);
		}
	}
}
