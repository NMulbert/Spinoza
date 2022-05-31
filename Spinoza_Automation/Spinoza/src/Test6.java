import java.util.concurrent.TimeUnit;
import org.openqa.selenium.By;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.chrome.ChromeDriver;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;
import org.testng.Assert;

public class Test6 {

	public static void main(String[] args) throws InterruptedException {
		// Invoke Browser
		// Tests Edit Question

		System.setProperty("webdriver.chrome.driver", ".\\lib\\chromedriver.exe");
		WebDriver driver = new ChromeDriver();
		driver.manage().timeouts().implicitlyWait(10, TimeUnit.SECONDS);

		driver.manage().window().maximize();
//		driver.get("http://localhost:3000/");
		driver.get("https://www.zionetapp.com/");
		driver.findElement(By.xpath("//span[contains(text(),'Questions')]")).click();
		
		System.out.println("TEST START - QUESTION'S EDIT");

		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");

		System.out.println("TEST START - TEST 1");
		EditQuestionTitle(driver);
		System.out.println("TEST END - TEST 1");

		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");

		System.out.println("TEST START - TEST 2");
		EditQuestionDescription(driver);
		System.out.println("TEST END - TEST 2");

		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");

		System.out.println("TEST START - TEST 3");
		EditQuestionTags(driver);
		System.out.println("TEST END - TEST 3");

		System.out.println("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");

		System.out.println("TEST END - QUESTION'S EDIT");

		Thread.sleep(5000);
		driver.quit();

	}

	public static void EditQuestionTitle(WebDriver driver) {
		try {
			// EDIT QUESTION TITLE

			// EDIT TEST
			EditTest(driver);
			Thread.sleep(3000);
			
			// EDIT TITLE
			WebElement Title = driver.findElement(By.xpath(
					"//input[@class='mantine-TextInput-defaultVariant mantine-TextInput-input mantine-us7vc8']"));
			Title.click();
			Title.clear();
			Title.sendKeys("NEW TITLE");
			System.out.println("TITLE = 'NEW TITLE'");

			// SAVE AS DRAFT
			driver.findElement(By.cssSelector(
					"button[class='mantine-Button-gradient mantine-Button-root mantine-Group-child mantine-1m2tbz']"))
					.click();

			WebDriverWait wait = new WebDriverWait(driver, 15);
			wait.until(ExpectedConditions.visibilityOfElementLocated(By.xpath("//div[@role='alert']")));

			String alert = driver.findElement(By.xpath("//div[@role='alert']")).getText();

			if (alert.contains("InternalServerError")) {
				System.out.println("TEST EDIT TITLE FAILED");
			} else {
				EditQuestionTitle_Check(driver);
			}
			
			Thread.sleep(500);
			driver.findElement(By.xpath("//span[contains(text(),'Questions')]")).click();

		} catch (Exception e) {
			System.out.println("Error = EditQuestionTitle");
			System.out.println(e);
			
		}
	}
	
	public static void EditQuestionDescription(WebDriver driver) {
		try {
			// EDIT TEST DESCRIPTION

			// EDIT TEST
			EditTest(driver);
			Thread.sleep(3000);

			// EDIT DESCRIPTION
			WebElement Description = driver.findElement(By.cssSelector(".w-md-editor-text-input"));
			Description.click();
			Description.clear();
			Description.sendKeys("NEW EDIT DESCRIPTION");
			System.out.println("DESCRIPTION = 'NEW EDIT DESCRIPTION'");

			// SAVE AS DRAFT
			driver.findElement(By.cssSelector(
					"button[class='mantine-Button-gradient mantine-Button-root mantine-Group-child mantine-1m2tbz']"))
					.click();

			WebDriverWait wait = new WebDriverWait(driver, 15);
			wait.until(ExpectedConditions.visibilityOfElementLocated(By.xpath("//div[@role='alert']")));

			String alert = driver.findElement(By.xpath("//div[@role='alert']")).getText();

			if (alert.contains("InternalServerError")) {
				System.out.println("TEST EDIT DESCRIPTION FAILED");
			} else {
				EditQuestionDescription_Check(driver);
			}

			driver.findElement(By.xpath("//span[contains(text(),'Tests')]")).click();

		} catch (Exception e) {
			System.out.println("Error = EditTestDescription");
			System.out.println(e);
		}

	}

	public static void EditQuestionTags(WebDriver driver) {
		try {
			// EDIT TAGS

			// EDIT TEST
			EditTest(driver);
			Thread.sleep(3000);

			// EDIT TAGS
			int TagsCount = driver.findElements((By.xpath(
					"//button[@class='mantine-ActionIcon-transparent mantine-ActionIcon-root mantine-MultiSelect-defaultValueRemove mantine-9kufq0']")))
					.size();

			if (TagsCount > 1) {
				driver.findElement(By.xpath(
						"(//button[@class='mantine-ActionIcon-transparent mantine-ActionIcon-root mantine-MultiSelect-defaultValueRemove mantine-9kufq0'])[1]"))
						.click();
				System.out.println("TAGS = " + (driver
						.findElement(By.cssSelector(".mantine-1kunnmy.mantine-MultiSelect-values")).getText()));
			}

			driver.findElement(By.cssSelector(".mantine-1kunnmy.mantine-MultiSelect-values")).click();

			// SAVE AS DRAFT
			driver.findElement(By.cssSelector(
					"button[class='mantine-Button-gradient mantine-Button-root mantine-Group-child mantine-1m2tbz']"))
					.click();

			WebDriverWait wait = new WebDriverWait(driver, 15);
			wait.until(ExpectedConditions.visibilityOfElementLocated(By.xpath("//div[@role='alert']")));

			String alert = driver.findElement(By.xpath("//div[@role='alert']")).getText();

			if (alert.contains("InternalServerError")) {
				System.out.println("TEST EDIT DESCRIPTION FAILED");
			} else {
				EditQuestionTags_Check(driver);
			}

			driver.findElement(By.xpath("//span[contains(text(),'Tests')]")).click();

		} catch (Exception e) {
			System.out.println("Error = EditTestTags");
			System.out.println(e);
		}
	}


	private static void EditTest(WebDriver driver) {
		try {
			// EDITE TEST
			driver.findElement(By.xpath("//span[contains(text(),'Questions')]")).click();
			Thread.sleep(3000);
			int QuestionCount = driver.findElements(By.xpath("//div[@class='mantine-Col-root mantine-1akkebb']")).size()
					- 1;
			System.out.println("TEST's COUNT = " + QuestionCount);

			driver.findElement(By.xpath("(//button[@class='mantine-Button-light mantine-Button-root mantine-5l7wha'])["
					+ (QuestionCount) + "]")).click();

			driver.findElement(By.xpath(".//*[@class='icon icon-tabler icon-tabler-edit']")).click();

		} catch (Exception e) {
			System.out.println("Error = EditTestButton");
			System.out.println(e);
		}
	}
	
	private static void EditQuestionTitle_Check(WebDriver driver) {
		try {
			Assert.assertTrue(driver.findElement(By.xpath("//div[@role='alert']")).getText().contains("updated"));
			System.out.println("Edit Test Title = SUCCESS");
		} catch (Exception e) {
			System.out.println("Edit Test Title = FAILED");
			System.out.println(e);
		}
	}
	
	private static void EditQuestionDescription_Check(WebDriver driver) {
		try {
			Assert.assertTrue(driver.findElement(By.xpath("//div[@role='alert']")).getText().contains("updated"));
			System.out.println("Edit Test Description = SUCCESS");
		} catch (Exception e) {
			System.out.println("Edit Test Description = FAILED");
			System.out.println(e);
		}
	}

	private static void EditQuestionTags_Check(WebDriver driver) {
		try {
			Assert.assertTrue(driver.findElement(By.xpath("//div[@role='alert']")).getText().contains("updated"));
			System.out.println("Edit Test Tags = SUCCESS");
		} catch (Exception e) {
			System.out.println("Edit Test Tags = FAILED");
			System.out.println(e);
		}
	}
}
