export const PublicatieStatus = Object.freeze({
  concept: "concept",
  gepubliceerd: "gepubliceerd",
  ingetrokken: "ingetrokken"
});

export type Publicatie = {
  uuid?: string;
  publisher: string;
  verantwoordelijke: string;
  officieleTitel: string;
  verkorteTitel: string;
  omschrijving: string;
  eigenaar?: Eigenaar;
  publicatiestatus: keyof typeof PublicatieStatus;
  registratiedatum?: string;
  laatstGewijzigdDatum?: string;
  informatieCategorieen: string[];
};

export type PublicatieDocument = {
  uuid: string;
  identifier: string;
  publicatie: string;
  officieleTitel: string;
  verkorteTitel?: string;
  omschrijving?: string;
  publicatiestatus: keyof typeof PublicatieStatus;
  creatiedatum: string;
  registratiedatum?: string;
  laatstGewijzigdDatum?: string;
  bestandsnaam: string;
  bestandsformaat: string;
  bestandsomvang: number;
};

type Eigenaar = {
  identifier: string;
  weergaveNaam: string;
};
