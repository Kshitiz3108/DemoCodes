import os
import fitz
import cv2
import pandas as pd
from app.utils.general import get_filename_ext_type, copy_data, pdffilter
from app.utils.pdfreader import PdfReader
from app.pipeline.vendor.pipeline import vendor, vendorConfig
from app.dla.layout_extractor import LayoutExtractor
from app.dla.utils import _merge_layout_result_and_raw_data
from app.core.config import Settings
from app.pipeline.common import Vendors
from app import platform_exception as exp
from app.custom_errors import customErrors
from app.wrapper_functions import func_detail_logger
from app.constant import func_error_format, func_info_format
from app.custom_logging import pdf_logger
from app.utils.image_processing import applying_image_processing_task
from app.utils.pipeline import generate_task_list
from app.ocr.text_detection import get_dataframe_from_ocr


class Pipeline:
    vendors = {"vendor": vendor}
    vendors_config = {"vendor": vendorConfig}

    def __init__(self, settings: Settings):
        self.layout_ext = LayoutExtractor(
            layout_model_config=settings.LAYOUT.__dict__,
            classlabel=settings.LAYOUT.LABELS,
            run_rule_engine=settings.LAYOUT.RUN_RULES,
        )
        self.use_cached = False

    def get_vendor_pipeline(self, vendor):
        return self.vendors.get(vendor, None)

    def get_vendor_config(self, vendor):
        return self.vendors_config.get(vendor, {})

    @func_detail_logger()
    def vendor_classification(self, pdfpath: str):
        try:
            from app.core.config import settings

            settings.INVOICE_TYPE = ""
            settings.INVOICE_BASED = ""
            with fitz.open(pdfpath) as doc:
                text = ""
                for page in doc:
                    text += page.get_text()

            for key, value in (settings.DOCUMENT_TYPE_DICT).items():
                for val in value:
                    if val in text:
                        settings.INVOICE_TYPE = key
                        break

            if settings.INVOICE_TYPE == "":
                settings.INVOICE_TYPE = settings.DEFAULT_INVOICE_TYPE

            for key, value in (settings.INVOICE_BASED_KEY_DICT).items():
                for val in value:
                    if val in text:
                        settings.INVOICE_BASED = key
                        break

            if settings.INVOICE_BASED == "":
                settings.INVOICE_BASED = settings.DEFAULT_INVOICE_BASED

            return "vendor"
        except exp.PlatformException as err:
            raise err
        except Exception as err:
            raise exp.PlatformException(err, customErrors.ER_VENDOR_CLASSIFICATION)

    @func_detail_logger()
    def extract_layout(
        self, pdfpath: str, save_path: str = None, intermediate_save: bool = False
    ):
        try:
            pdf_logger.info(
                func_info_format(
                    "extract_layout", f"Extracting layout data : {pdfpath}"
                )
            )
            save_images = True if intermediate_save and save_path else False
            df = self.layout_ext.run_inference_from_pdf(
                pdfpath=pdfpath, save_path=save_path, save_images=save_images
            )
            df.dropna(inplace=True)
            pdf_logger.info(
                func_info_format(
                    "extract_layout", f"Done extracting layout data : {pdfpath}"
                )
            )
            if intermediate_save and save_path:
                df.to_csv(f"{save_path}/layout.csv", index=False)
            return df
        except exp.PlatformException as err:
            raise err
        except PermissionError as err:
            raise exp.PlatformException(err, customErrors.ER_CREATE_FILE_PERMISSION_DN)
        except Exception as err:
            raise exp.PlatformException(err, customErrors.ER_EXTRACT_LAYOUT_UN)

    @func_detail_logger()
    def extract_raw_data_from_text_based_pdf(
        self, pdfpath: str, save_path: str = None, intermediate_save: bool = False
    ):
        try:
            pdf_logger.info(
                func_info_format(
                    "extract_raw_data_from_text_based_pdf",
                    f"Extracting raw data : {pdfpath}",
                )
            )
            reader = PdfReader(data=pdfpath, stream=False)
            df, _, type = reader.get_data(with_images=False)
            df.dropna(inplace=True)
            pdf_logger.info(
                func_info_format(
                    "extract_raw_data_from_text_based_pdf",
                    f"Done extracting raw data : {pdfpath}",
                )
            )
            if intermediate_save and save_path:
                df.to_csv(f"{save_path}/raw.csv", index=False)
            return df, type
        except exp.PlatformException as err:
            raise err
        except PermissionError as err:
            raise exp.PlatformException(err, customErrors.ER_CREATE_FILE_PERMISSION_DN)
        except Exception as err:
            raise exp.PlatformException(
                err, customErrors.ER_EXTRACT_RAW_DATA_FROM_TEXT_BASED_PDF_UN
            )

    @func_detail_logger()
    def extract_raw_data_from_img(self, pdfpath, save_path, intermediate_save):
        try:
            from app.core.config import settings

            image = cv2.imread(pdfpath)
            if settings.IMAGE_PROCESSING_SERVICE:
                ALL_IMAGE_PROCESSING = generate_task_list()
                image = applying_image_processing_task(
                    image=image, ALL_IMAGE_PROCESSING=ALL_IMAGE_PROCESSING
                )
            raw_df = get_dataframe_from_ocr(image=image, page_no=1)
            pdf_logger.info(
                func_info_format(
                    "extract_raw_data_from_img", f"Done extracting raw data : {pdfpath}"
                )
            )
            if intermediate_save and save_path:
                raw_df.to_csv(f"{save_path}/raw.csv", index=False)
            return raw_df
        except exp.PlatformException as err:
            raise err
        except PermissionError as err:
            raise exp.PlatformException(err, customErrors.ER_CREATE_FILE_PERMISSION_DN)
        except Exception as err:
            raise exp.PlatformException(
                err, customErrors.ER_EXTRACT_RAW_DATA_FROM_IMG_BASED_PDF_UN
            )

    @func_detail_logger()
    def extract_raw_data_from_img_based_pdf(
        self, pdfpath: str, save_path: str = None, intermediate_save: bool = False
    ):
        try:
            pdf_logger.info(
                func_info_format(
                    "extract_raw_data_from_img_based_pdf",
                    f"Extracting raw data : {pdfpath}",
                )
            )
            # image = convert_pdf_to_images(pdfpath)
            reader = PdfReader(data=pdfpath)
            images = reader.get_images()

            raw_df = pd.DataFrame()
            from app.core.config import settings

            for pageno, img in images.items():
                pdf_logger.info(
                    func_info_format(
                        "extract_raw_data_from_img_based_pdf",
                        f"Process is running for page {pageno}",
                    )
                )
                if settings.IMAGE_PROCESSING_SERVICE:
                    ALL_IMAGE_PROCESSING = generate_task_list()
                    img = applying_image_processing_task(
                        image=img, ALL_IMAGE_PROCESSING=ALL_IMAGE_PROCESSING
                    )

                df = get_dataframe_from_ocr(image=img, page_no=pageno)
                raw_df = pd.concat([raw_df, df], ignore_index=True)
            pdf_logger.info(
                func_info_format(
                    "extract_raw_data_from_img_based_pdf",
                    f"Done extracting raw data : {pdfpath}",
                )
            )
            if intermediate_save and save_path:
                raw_df.to_csv(f"{save_path}/raw.csv", index=False)
            return raw_df
        except exp.PlatformException as err:
            raise err
        except PermissionError as err:
            raise exp.PlatformException(err, customErrors.ER_CREATE_FILE_PERMISSION_DN)
        except Exception as err:
            raise exp.PlatformException(
                err, customErrors.ER_EXTRACT_RAW_DATA_FROM_IMG_BASED_PDF_UN
            )

    @func_detail_logger()
    async def extract_from_document(
        self,
        pdfpath: str,
        save_path=None,
        intermediate_save: bool = False,
    ):
        pdf_logger.info(
            func_info_format("extract_from_document", f"Running for pdf : {pdfpath}")
        )
        _, name, ext, typ = get_filename_ext_type(pdfpath)
        save_path = f"{save_path}/{name}" if save_path else None
        try:
            if save_path:
                os.makedirs(save_path, exist_ok=True)
                copy_data(src=pdfpath, dst=save_path)
            layout_csv_path = f"{save_path}/temp.csv"
            if self.use_cached and save_path and os.path.exists(layout_csv_path):
                pdf_logger.info(
                    func_info_format(
                        "extract_from_document",
                        f"Using cached file for Layout Extractor : {layout_csv_path}",
                    )
                )
            else:
                if ext == "pdf" and typ == "text":
                    raw_df, type = self.extract_raw_data_from_text_based_pdf(
                        pdfpath=pdfpath,
                        save_path=save_path,
                        intermediate_save=intermediate_save,
                    )
                elif ext == "pdf" and typ == "image":
                    type = "image_type"
                    raw_df = self.extract_raw_data_from_img_based_pdf(
                        pdfpath=pdfpath,
                        save_path=save_path,
                        intermediate_save=intermediate_save,
                    )
                elif ext in ["jpg", "png", "jpeg", "tiff"]:
                    type = "image_type"
                    raw_df = self.extract_raw_data_from_img(
                        pdfpath=pdfpath,
                        save_path=save_path,
                        intermediate_save=intermediate_save,
                    )
                else:
                    raise exp.PlatformException(
                        "DOCUMENT IS INVALID", customErrors.ER_INVALID_DOCUMENT
                    )
                layout_df = self.extract_layout(
                    pdfpath=pdfpath,
                    save_path=save_path,
                    intermediate_save=intermediate_save,
                )
                df = _merge_layout_result_and_raw_data(ldf=layout_df, rdf=raw_df)
                df.to_csv(layout_csv_path, index=False)

            vendorName = self.vendor_classification(pdfpath)
            vendorPipeline: Vendors = self.get_vendor_pipeline(vendorName)
            vendorConfig = self.get_vendor_config(vendorName)
            if vendorPipeline:
                vendorPipeline(vendorConfig()).convert_pdf_to_csv(
                    layout_csv_path, pdfpath, save_path, type
                )
            else:
                raise exp.PlatformException(
                    "VENDOR DOES NOT EXISTS", customErrors.ER_INVALID_DOCUMENT_TYPE
                )
        except exp.PlatformException as err:
            raise err
        except PermissionError as err:
            raise exp.PlatformException(err, customErrors.ER_CREATE_FILE_PERMISSION_DN)
        except Exception as err:
            if save_path:
                with open(f"{save_path}/error.txt", "w", encoding="utf-8") as fp:
                    fp.write(traceback.format_exc())
            raise exp.PlatformException(err, customErrors.ER_EXTRACT_FROM_DOCUMENT_UN)

    @func_detail_logger()
    def extract_from_folder(
        self, folderpath: str, save_path=None, intermediate_save: bool = False
    ):
        try:
            files = os.listdir(folderpath)
            files = list(filter(pdffilter, files))
            failed = []
            for file in files:
                pdfpath = os.path.join(folderpath, file)
                try:
                    result = self.extract_from_document(
                        pdfpath=pdfpath,
                        save_path=save_path,
                        intermediate_save=intermediate_save,
                    )
                    pdf_logger.info(
                        func_info_format(
                            "extract_from_folder", f"Result for pdf: {file} - {result}"
                        )
                    )
                except Exception as err:
                    pdf_logger.error(
                        func_error_format(
                            "extract_from_folder",
                            [],
                            {},
                            f"ERROR : {err}",
                        )
                    )
                    failed.append((file, str(err)))
            pdf_logger.error(
                func_error_format(
                    "extract_from_document", [], {}, f"Failed pdfs : {failed}"
                )
            )
        except exp.PlatformException as err:
            raise err
        except Exception as err:
            raise exp.PlatformException(err, customErrors.ER_EXTRACT_FROM_FOLDER_UN)
